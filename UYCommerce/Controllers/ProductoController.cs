using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UyCommerce.Application.Contracts.Persistance.Services;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Domain.Exceptions;
using UyCommerce.Application.Common.Enums;
using UYCommerce.ViewModels;
using UyCommerce.Application.DTOs.Productos;
using UyCommerce.Application.DTOs.Images;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace UYCommerce.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IFileService _fileService;
        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;
        private readonly IMarcaService _marcaService;

        public ProductoController(IFileService fileService, IProductoService productoService,
            ICategoriaService categoriaService, IMarcaService marcaService)
        {
            _fileService = fileService;
            this._productoService = productoService;
            this._categoriaService = categoriaService;
            this._marcaService = marcaService;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> CrearProducto()
        {
            ProductoModel model = new();
            await GetSelectsForCrearProducto(model);
            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> CrearProducto(ProductoModel producto)
        {
            string error;
            if (ModelState.IsValid)
            {
                try
                {
                    AddProductoRequest newProducto = new AddProductoRequest()
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
                            .Select(i => new CreateImageRequest() { ImageName = i.FileName, Stream = i.OpenReadStream() })
                            .ToList();
                        newProducto.Imagenes = imagenes;
                    }

                    await _productoService.AddProducto(newProducto);
                    return Redirect("/admin/productos");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    error = e.Message;
                }
                catch (NotFoundException e)
                {
                    Response.StatusCode = 404;
                    error = e.Message;
                }
                catch (Exception)
                {
                    Response.StatusCode = 500;
                    error = "Hubo un error al guardar el producto, intente denuevo mas tarde.";
                }
                ModelState.AddModelError("", error);
            }

            await GetSelectsForCrearProducto(producto);

            return View(producto);
        }

        private async Task GetSelectsForCrearProducto(ProductoModel model)
        {
            var categorias = (await _categoriaService.GetCategorias()).Categorias;
            var marcas = (await _marcaService.GetMarcas()).Marcas;
            model.Categorias = new SelectList(categorias, "Id", "Nombre");
            model.Marcas = new SelectList(marcas, "Id", "Nombre");
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> AgregarSku(int productoId)
        {
            try
            {
                var producto = await _productoService.GetProductoCompleteByIdNoTracking(productoId);

                AgregarSkuModel skuModel = new()
                {
                    ProductoId = producto.Id,
                    Atributos = producto.Atributos,
                    Producto = producto,
                };

                return View(skuModel);
            }
            catch (Exception){
                Response.StatusCode = 500;
            }

            return Redirect("/admin/productos");
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> AgregarSku(AgregarSkuModel skuModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    List<CreateImageRequest> imagenes = [];
                    skuModel.AtributoValores ??= [];

                    if (skuModel.Imagenes != null)
                    {
                        imagenes = [.. skuModel.Imagenes.Select(i => new CreateImageRequest() { ImageName = i.Name, Stream = i.OpenReadStream() })];
                    }

                    AddSkuAProductoRequest skuDto = new()
                    {
                        AtributoValores = (List<int>)skuModel.AtributoValores,
                        Imagenes = imagenes,
                        Nombre = skuModel.Nombre,
                        Precio = skuModel.Precio,
                        ProductoId = skuModel.ProductoId,
                        Stock = skuModel.Stock
                    };

                    await _productoService.AddSkuAProducto(skuDto);
                    var producto = await _productoService.GetProductoCompleteByIdNoTracking(skuModel.ProductoId);
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

        [HttpGet]
        [Route("/search")]
        public async Task<IActionResult> BuscarProductosPorQuery(string query, int orden = 1, int pag = 1, int qty = 3)
        {
            GetProductosResponse result = new();

            if (pag < 1 || string.IsNullOrEmpty(query) || orden < 1 || orden > 3 || qty < 3)
            {
                Response.StatusCode = 400;
            }
            else
            {
                try
                {
                    var userId = GetUserId();
                    result = await _productoService.GetProductosByQuery(query, (ProductsOrderBy)orden, pag, qty, userId);
                    return View("BusquedaProductos", result);
                }
                catch (Exception)
                {
                    Response.StatusCode = 500;
                }
            }
            return View("NotFound");
        }

        [HttpGet]
        [Route("/c/{categoria}")]
        public async Task<IActionResult> BuscarProductosPorCategoria(string categoria, string[]? filtros, int orden = 1, int pag = 1, int qty = 3)
        {
            try
            {
                var userId = GetUserId();
                var result = await _productoService.GetProductosByCategoria(categoria, filtros, userId, orden, pag, qty);
                return View("BusquedaProductos", result);
            }
            catch (NotFoundException)
            {
                Response.StatusCode = 404;
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
            }
            return View("NotFound");
        }

        private int? GetUserId()
        {
            if (int.TryParse(User?.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return userId;
            return null;
        }

        [HttpGet]
        [Route("/{key}")]
        public async Task<IActionResult> GetProductoPorKey(string key)
        {
            try
            {
                var correctSkuKey = await _productoService.FindSkuKeyMatch(key);
                if (!string.IsNullOrEmpty(correctSkuKey) && correctSkuKey != key)
                {
                    return Redirect($"/{correctSkuKey}");
                }
                var userAuthenticated = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId);
                var result = await _productoService.GetSkuByKey(correctSkuKey, userAuthenticated ? userId : null);
                result.IsAuthenticated = userAuthenticated;
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
            var productos = await _productoService.GetProductosByQueryName(query);
            return Json(productos);
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> EliminarSkuProducto([FromBody] int skuId)
        {
            try
            {
                await _productoService.DeleteSku(skuId);
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

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> EliminarProducto([FromBody] int productoId)
        {
            try
            {
                await _productoService.DeleteProducto(productoId);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (ArgumentOutOfRangeException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

            return Ok();
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Feature(int productId, bool featured)
        {
            try
            {
                await _productoService.FeatureProduct(productId, featured);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            return Ok(new { msg = $"Producto cambio su estado a {featured}." });
        }
    }
}


