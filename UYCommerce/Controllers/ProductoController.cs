using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Cms;
using UYCommerce.Data;
using UYCommerce.DTOs;
using UYCommerce.Models;
using UYCommerce.Services;
using UYCommerce.ViewModels;

namespace UYCommerce.Controllers
{
    public class ProductoController : Controller
    {
        private readonly ShopContext _context;
        private readonly IFileService _fileService;

        public ProductoController(ShopContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
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

                var productoDb = await _context.Productos.FirstOrDefaultAsync(p => p.Nombre!.ToLower() == producto.Nombre.ToLower());

                if (productoDb is not null)
                {
                    ViewBag.msj = "Ese producto ya existe";
                    return View(producto);
                }

                var categoria = await _context.Categorias.Include(c => c.Atributos).FirstOrDefaultAsync(c => c.Id == producto.CategoriaId);
                var marca = await _context.Marcas.FirstOrDefaultAsync(m => m.Id == producto.MarcaId);

                if (categoria is null)
                {
                    ViewBag.msj = "Error";
                    return View(producto);
                }

                var atributos = producto.Atributos is not null ?
                    categoria.Atributos?.Where(a => producto.Atributos.Any(x => a.Id == x)).ToList()! : null;

                Producto productoNuevo = new()
                {
                    Nombre = producto.Nombre,
                    NombreClave = producto.Nombre.ToLower().Replace(" ", "-"),
                    Descripcion = producto.Descripcion,
                    Imagenes = producto.Imagenes.Select(i => new ProductoImagen { ImagenNombre = i.FileName }).ToList(),
                    Categoria = categoria,
                    Marca = marca,
                    Atributos = atributos
                };

                await _context.Productos.AddAsync(productoNuevo);
                await _context.SaveChangesAsync();

                await _fileService.SaveFile(producto.Imagenes!.ToList(), "Imagenes/Productos");

                return Redirect("/admin/productos");
            }

            return View(producto);
        }



        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> AgregarSku(int productoId)
        {

            var producto = await GetProductoById(productoId);

            if (producto is null || !producto.Atributos!.Any() && producto.Skus!.Any())
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

            Producto? producto = null;

            if (ModelState.IsValid)
            {
                producto = await GetProductoById(skuModel.ProductoId);

                if (producto is null)
                    return View(skuModel);

                var puedeAgregarSku = ValidarAgregarSku(producto, skuModel);

                if (!puedeAgregarSku)
                {
                    ViewBag.error = "Algo salio mal";
                    return View(skuModel);
                }

                string key = producto.NombreClave!;

                List<AtributoValor> atributos = new();

                if (skuModel.AtributoValores is not null && skuModel.AtributoValores!.Any() && producto.Atributos is not null)
                    atributos = producto.Atributos.SelectMany(a => a.Valores!).Where(a => skuModel.AtributoValores.Any(av => av == a.Id)).ToList();

                if (atributos.Any()) atributos.ForEach(a => key += "-" + a.Valor);

                Sku nuevoSku = new()
                {
                    ProductoId = producto.Id,
                    Key = key.ToLower(),
                    Nombre = skuModel.Nombre,
                    Precio = skuModel.Precio,
                    Imagenes = skuModel.Imagenes is not null ?
                    skuModel.Imagenes.Select(i => new SkuImagen { ImagenNombre = i.FileName }).ToList() :
                    producto.Imagenes?.Select(i => new SkuImagen { ImagenNombre = i.ImagenNombre }).ToList(),
                    AtributosValores = atributos,
                    Stock = skuModel.Stock
                };

                await _fileService.SaveFile(skuModel.Imagenes!.ToList(), "Imagenes/Productos");

                await _context.AddAsync(nuevoSku);
                await _context.SaveChangesAsync();

                return Redirect("/admin/productos");

            }

            skuModel.Atributos = producto?.Atributos;

            return View(skuModel);

        }

        private static bool ValidarAgregarSku(Producto producto, AgregarSkuModel sku)
        {

            if (producto.Atributos!.Any() && sku.AtributoValores is null)
            {
                return false;
            }

            bool skuAtributosExisten = producto.Skus!.Any(s => s.AtributosValores!.All(a => sku.AtributoValores!.Any(av => av == a.Id)));

            if (skuAtributosExisten)
            {
                return false;
            }

            if (producto.Skus!.Any(s => s.Nombre.ToLower() == sku.Nombre.ToLower()))
            {
                return false;
            }

            return true;
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
        public async Task<IActionResult> BuscarProductosPorQuery(string query, int orden = 0, int pag = 0, int qty = 3)
        {
            BusquedaProductoVM result = new();

            result.Skus = await GetSkusByQuery(query);

            //paginacion
            result.NumberOfPages = decimal.Divide(result.Skus.Count, qty);
            int numPag = pag > 0 ? pag - 1 : 0;

            if (orden > 0)
                result.Skus = OrdenarSkus(result.Skus, orden);

            result.Skus = result.Skus.Skip(numPag * qty).Take(qty).ToList();

            result.Categorias = await _context.Categorias.Where(c => c.CategoriaPadre == null)
                .Select(x => new Categoria { Id = x.Id, Nombre = x.Nombre }).ToListAsync();

            if (User.Identity!.IsAuthenticated)
                result.Favoritos = _context.Usuarios.Where(u => u.Id.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier)).SelectMany(u => u.Favoritos!).ToList();

            result.Pag = numPag + 1;

            return View("BusquedaProductos", result);
        }

        private async Task<ICollection<Sku>> GetSkusByQuery(string query) {

            IQueryable<Producto> productos = GetProductosQueryable();

            productos = productos.Where(p => p.Nombre!.ToLower().StartsWith(query.ToLower()));

            var skus = await productos.SelectMany(p => p.Skus!).ToListAsync();

            return skus;
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
                    result.Categoria.SubCategorias?.OrderBy(c=> c.Id).ToList() :
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

        /// <summary>
        /// Encuentra el producto segun su NombreClave (key) y segun las opciones seleccionadas del cliente.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="options"></param>
        /// <returns>verProductoViewModel</returns>
        [HttpGet]
        [Route("/{key}")]
        public async Task<IActionResult> GetProductoPorKey(string key)
        {

            var sku = await _context.Skus
                .Include(s => s.AtributosValores)!.ThenInclude(av => av.Atributo)
                .Include(s => s.Producto).ThenInclude(p => p!.Reviews)!.ThenInclude(r => r.Usuario)
                .Include(s => s.Producto).ThenInclude(p => p!.Preguntas)
                .Include(s => s.Producto).ThenInclude(p => p!.Reviews)
                .Include(s => s.Producto).ThenInclude(p => p!.Marca)
                .Include(s => s.Producto).ThenInclude(p => p!.Categoria).ThenInclude(c => c!.CategoriaPadre)
                .Include(s => s.Imagenes)
                .FirstOrDefaultAsync(s => s.Key.ToLower().Contains(key.ToLower()));

            if (sku is not null)
            {
                var skus = _context.Skus
                    .Where(s => s.ProductoId == sku.ProductoId)
                    .Include(s => s.AtributosValores)!.ThenInclude(a => a.Atributo)
                    .Include(s => s.Producto!.Categoria!.Productos)!.ThenInclude(p => p.Skus)!.ThenInclude(s => s.Imagenes)
                    .ToList();

                VerProductoVM result = new()
                {
                    Sku = sku,
                    Reviews = sku.Producto!.Reviews,
                    Categoria = sku.Producto.Categoria,
                    ProductosRelacionados = sku.Producto.Categoria!.Productos!.SelectMany(p => p.Skus!).ToList(),
                };

                result.Favoritos = _context.Usuarios.Where(u => u.Id.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier)).SelectMany(u => u.Favoritos!).ToList();

                if (sku.AtributosValores != null && sku.AtributosValores.Any())
                {
                    List<AtributoValor> opciones = skus.SelectMany(s => s.AtributosValores!.Where(a => a.Atributo!.Id == sku.AtributosValores!.First().Atributo!.Id)).ToList();

                    opciones.AddRange(skus.Where(s => s.AtributosValores!.Contains(sku.AtributosValores!.First())).SelectMany(s => s.AtributosValores!).ToList());

                    opciones = opciones.Distinct().ToList();
                    var opcionesPorAtributo = opciones.GroupBy(a => a.Atributo);

                    result.Opciones = opcionesPorAtributo!;
                }

                return View("VerProducto", result);
            }

            return Redirect("/NotFound");
        }

        [HttpPost]
        public async Task<JsonResult> Autocomplete(string query)
        {

            var productos = await _context.Productos
                .Include(p => p.Imagenes)
                .Where(p => p.Nombre!.ToLower().StartsWith(query.ToLower())
                || p.Nombre.ToLower().Contains(query.ToLower()))
                .Select(p => new
                {
                    id = p.Id,
                    nombre = p.Nombre,
                    nombreclave = p.NombreClave,
                    img = p.Imagenes!.First().ImagenNombre
                }).ToListAsync();

            return Json(productos);
        }

        

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> EliminarSkuProducto([FromBody] int skuId)
        {

            var sku = await GetSkuById(skuId);

            if (sku is null)
                return BadRequest(new { error = "Error sku not found" });

            if (await SkuInUse(skuId))
                return BadRequest(new { error = "Error sku in use" });

            try
            {
                _context.Remove(sku);
                await _context.SaveChangesAsync();

            }
            catch (Exception e)
            {
                return StatusCode(500);
            }

            return Ok();
        }

        private async Task<Sku?> GetSkuById(int skuId)
        {

            var sku = await _context.Skus
                .Include(s => s.Imagenes)
                .Include(s => s.Usuarios)
                .Include(s => s.AtributosValores)
                .FirstOrDefaultAsync(s => s.Id == skuId);

            return sku;
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


            var product = await GetProductoById(productId);

            if (product is null)
                return BadRequest(new { msg = "Product not found" });

            product.Featured = featured;

            _context.Update(product);
            await _context.SaveChangesAsync();

            return Ok(new { msg = featured == true ? "Featured" : "Not featured" });
        }
    }
}


