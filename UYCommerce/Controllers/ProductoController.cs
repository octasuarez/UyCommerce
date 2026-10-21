using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UyCommerce.Infrastructure.Persistance;
using UyCommerce.Domain.Entities;
using UyCommerce.Application.Contracts.Persistance.Services;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Application.DTOs;
using UyCommerce.Domain.Exceptions;
using UyCommerce.Application.Common.Enums;
using UYCommerce.ViewModels;

namespace UYCommerce.Controllers
{
    public class ProductoController : Controller
    {
        private readonly ShopContext _context;
        private readonly IFileService _fileService;
        private readonly IProductoService _productoService;

        public ProductoController(ShopContext context, IFileService fileService, IProductoService productoService)
        {
            _context = context;
            _fileService = fileService;
            this._productoService = productoService;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        [Authorize(Policy = "Admin")]
        public IActionResult CrearProducto()
        {

            return View();
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> CrearProducto(ProductoModel producto)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    AddProductoDto newProducto = new AddProductoDto()
                    {
                        Nombre = producto.Nombre,
                        Descripcion = producto.Descripcion,
                        CategoriaId = producto.CategoriaId,
                        MarcaId = producto.MarcaId,
                        Atributos = producto.Atributos ?? []
                    };

                    if (producto.Imagenes.Any())
                    {
                        var imagenes = producto.Imagenes
                            .Select(i => new CreateImageDto() { ImageName = i.Name, Stream = i.OpenReadStream() })
                            .ToList();
                        newProducto.Imagenes = imagenes;
                    }

                    await _productoService.AddProductoAsync(newProducto);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Response.StatusCode = 400;
                }
                catch (NotFoundException e)
                {
                    Response.StatusCode = 404;
                }
                catch (Exception e)
                {
                    Response.StatusCode = 500;
                }

                return Redirect("/admin/productos");
            }

            return View(producto);
        }



        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> AgregarSku(int productoId)
        {

            var producto = await _productoService.GetProductoCompleteByIdNoTrackingAsync(productoId);

            if (producto is null || producto.Atributos!.Count == 0 && producto.Skus!.Count != 0)
            {
                return Redirect("/productos");
            }

            AgregarSkuModel skuModel = new()
            {
                ProductoId = producto.Id,
                Atributos = producto.Atributos,
                Producto = producto,
            };

            return View(skuModel);
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> AgregarSku(AgregarSkuModel skuModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    List<CreateImageDto> imagenes = [];
                    skuModel.AtributoValores ??= [];

                    if (skuModel.Imagenes != null)
                    {
                        imagenes = [.. skuModel.Imagenes.Select(i => new CreateImageDto() { ImageName = i.Name, Stream = i.OpenReadStream() })];
                    }

                    AddSkuAProductoDto skuDto = new()
                    {
                        AtributoValores = (List<int>)skuModel.AtributoValores,
                        Imagenes = imagenes,
                        Nombre = skuModel.Nombre,
                        Precio = skuModel.Precio,
                        ProductoId = skuModel.ProductoId,
                        Stock = skuModel.Stock
                    };

                    await _productoService.AddSkuAProductoAsync(skuDto);
                    var producto = await _productoService.GetProductoCompleteByIdNoTrackingAsync(skuModel.ProductoId);
                    skuModel.Atributos = producto?.Atributos;
                }
                catch (Exception e)
                {
                    Response.StatusCode = 400;
                    ViewBag.error = e.Message;
                    return View(skuModel);
                }
                return Redirect("/admin/productos");
            }
            return View(skuModel);
        }

        private async Task<Producto?> GetProductoById(int id)
        {

            var producto = await _context.Productos
                .Include(p => p.Atributos)!.ThenInclude(a => a.Valores)
                .Include(p => p.Reviews)
                .Include(p => p.Skus)
                .Include(p => p.Skus)!.ThenInclude(s => s.Imagenes)
                .Include(p => p.Imagenes)
                .FirstOrDefaultAsync(p => p.Id == id);

            return producto;
        }

        [HttpGet]
        [Route("/search")]
        public async Task<IActionResult> BuscarProductosPorQuery(string query, int orden = 1, int pag = 1, int qty = 3)
        {
            BusquedaProductoVM result = new();

            if (pag < 1 || string.IsNullOrEmpty(query) || orden < 1 || orden > 3 || qty < 3)
            {
                Response.StatusCode = 400;
                return View("BusquedaProductos", result);
            }
            try
            {
                int? userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int id) ? id : null;
                var queryResult = await _productoService.GetProductosByQueryAsync(query, (ProductsOrderBy)orden, pag - 1, qty, userId);
                result.Skus = queryResult.Items;
                result.NumberOfPages = (queryResult.TotalItems + qty - 1) / qty;
                result.Pag = pag;
                result.Categorias = queryResult.Categorias;
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
            }
            return View("BusquedaProductos", result);
        }

        [HttpGet]
        [Route("/c/{categoria}")]
        public async Task<IActionResult> BuscarProductosPorCategoria(string categoria, string[]? filtros, int orden = 0, int pag = 0, int qty = 3)
        {
            BusquedaProductoVM result = new()
            {
                Categoria = await _context.Categorias
                .Include(c => c.CategoriaPadre).ThenInclude(p => p!.SubCategorias!)
                .Include(c => c.Atributos)!.ThenInclude(a => a.Valores)
                .Include(c => c.SubCategorias)
                .FirstOrDefaultAsync(c => c.Nombre!.ToLower() == categoria.ToLower())
            };

            if (result.Categoria is not null)
            {
                result.Skus = await GetSkusByCategoria(categoria.ToLower());

                var atributosValores = filtros is not null && filtros.Any() ? GetAtributosDeFiltros(filtros) : null;

                if (atributosValores is not null)
                {
                    result.Skus = result.Skus.Where(s => atributosValores.All(atr => s.AtributosValores!.Any(av => av.Atributo!.Nombre!.ToLower() == atr.Key && av.Valor!.ToLower() == atr.Value))).ToList();
                    result.Atributos = atributosValores;
                    result.Filtros = result.Skus.SelectMany(s => s.AtributosValores!).Distinct().GroupBy(s => s.Atributo)!;
                }
                else
                {
                    result.Filtros = result.Skus.SelectMany(s => s.AtributosValores!).Distinct().GroupBy(s => s.Atributo)!;
                    result.Skus = result.Skus.DistinctBy(s => s.ProductoId).ToList();
                }

                result.NumberOfPages = decimal.Divide(result.Skus.Count, qty);
                var numPag = pag > 0 ? pag - 1 : 0;

                if (orden > 0)
                    result.Skus = OrdenarSkus(result.Skus, orden);

                result.Skus = result.Skus.Skip(numPag * qty).Take(qty).ToList();
                result.Categorias = await _context.Categorias.Where(c => c.MostrarEnInicio).Select(x => new Categoria { Id = x.Id, Nombre = x.Nombre }).ToListAsync();
                result.Favoritos = _context.Usuarios.Where(u => u.Id.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier)).SelectMany(u => u.Favoritos!).ToList();
                result.Pag = numPag + 1;
                result.SubCategorias = result.Categoria.SubCategorias!.Any() ?
                    result.Categoria.SubCategorias?.OrderBy(c => c.Id).ToList() :
                    result.Categoria.CategoriaPadre?.SubCategorias?.OrderBy(c => c.Id).ToList();

                return View("BusquedaProductos", result);
            }
            return View("NotFound");
        }

        private async Task<ICollection<Sku>> GetSkusByCategoria(string categoria)
        {

            IQueryable<Producto> productos = GetProductosQueryable();

            productos = productos.Where(p => p.Categoria!.Nombre!.ToLower() == categoria.ToLower()
                || p.Categoria.CategoriaPadre!.Nombre!.Equals(categoria));

            return await productos.SelectMany(p => p.Skus!).ToListAsync();
        }

        private IQueryable<Producto> GetProductosQueryable()
        {

            IQueryable<Producto> productos = _context.Productos
                 .Include(p => p.Categoria)
                 .ThenInclude(c => c!.CategoriaPadre)
                 .Include(p => p.Imagenes)
                 .Include(p => p.Skus)!.ThenInclude(s => s.Imagenes)
                 .Include(p => p.Skus)!.ThenInclude(s => s.Producto).ThenInclude(p => p!.Reviews)
                 .Include(p => p.Skus)!.ThenInclude(s => s.AtributosValores)!.ThenInclude(atv => atv.Atributo);

            return productos;
        }

        private static ICollection<Sku> OrdenarSkus(ICollection<Sku> skus, int orden)
        {

            switch (orden)
            {
                case 1:
                    skus = skus.OrderByDescending(s => s.Precio).ToList();
                    break;
                case 2:
                    skus = skus.OrderBy(s => s.Precio).ToList();
                    break;
                case 3:
                    skus = skus.OrderByDescending(s => s.Producto!.GetPuntuacionPromedio()).ToList();
                    break;
                default:
                    break;
            }

            return skus;
        }

        private static List<KeyValuePair<string, string>> GetAtributosDeFiltros(string[] filtros)
        {
            List<KeyValuePair<string, string>> atributosValores = new();

            for (int i = 0; i < filtros.Length; i++)
            {
                if (filtros[i].Contains('-'))
                {
                    var atributoValor = filtros[i].Split("-");
                    atributosValores.Add(new KeyValuePair<string, string>(atributoValor[0].ToLower(), atributoValor[1].ToLower()));
                }
            }
            return atributosValores;
        }

        [HttpGet]
        [Route("/{key}")]
        public async Task<IActionResult> GetProductoPorKey(string key)
        {
            try
            {
                var correctSkuKey = await _productoService.FindSkuKeyMatchAsync(key);
                if (!string.IsNullOrEmpty(correctSkuKey) && correctSkuKey != key)
                {
                    return Redirect($"/{correctSkuKey}");
                }
                var userAuthenticated = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId);
                var result = await _productoService.GetSkuByKeyAsync(correctSkuKey, userAuthenticated ? userId : null);

                return View("VerProducto", result);
            }
            catch (NotFoundException e)
            {
                Response.StatusCode = 400;
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
            }
            return Redirect("/NotFound");
        }

        [HttpPost]
        public async Task<JsonResult> Autocomplete(string query)
        {
            var productos = await _productoService.GetProductosByQueryNameAsync(query);
            return Json(productos);
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> EliminarSkuProducto([FromBody] int skuId)
        {
            try
            {
                await _productoService.DeleteSkuAsync(skuId);
            }
            catch (NotFoundException e)
            {
                Response.StatusCode = 400;
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
            }

            return Ok();
        }

        private async Task<bool> SkuInUse(int skuId)
        {

            var productoOrden = await _context.Ordenes.FirstOrDefaultAsync(o => o.Productos!.Any(p => p.SkuId == skuId));
            var productoCarrito = await _context.Carritos.FirstOrDefaultAsync(c => c.Productos!.Any(p => p.SkuId == skuId));

            if (productoOrden is not null || productoCarrito is not null)
                return true;

            return false;
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> EliminarProducto([FromBody] int productoId)
        {

            var producto = await GetProductoById(productoId);

            if (producto is null)
                return BadRequest(new { error = "Producto not found" });

            foreach (var s in producto.Skus!)
            {
                var skuInUse = await SkuInUse(s.Id);
                if (skuInUse)
                    return BadRequest(new { error = "Product has skus in use" });
            }

            try
            {
                _context.Remove(producto);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }

            return Ok();
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Feature(int productId, bool featured)
        {
            try
            {
                await _productoService.FeatureProductAsync(productId, featured);
            }
            catch (NotFoundException e)
            {
                Response.StatusCode = 400;
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
            }
            return Ok(new { msg = $"Producto cambio su estado a {featured}."});
        }
    }
}


