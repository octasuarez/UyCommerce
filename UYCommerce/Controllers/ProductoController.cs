using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IFileService _fileService;
        private readonly IProductoService _productoService;

        public ProductoController(IFileService fileService, IProductoService productoService)
        {
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

        [HttpGet]
        [Route("/search")]
        public async Task<IActionResult> BuscarProductosPorQuery(string query, int orden = 1, int pag = 1, int qty = 3)
        {
            GetProductosDto result = new();

            if (pag < 1 || string.IsNullOrEmpty(query) || orden < 1 || orden > 3 || qty < 3)
            {
                Response.StatusCode = 400;
            }
            else
            {
                try
                {
                    var userId = GetUserId();
                    result = await _productoService.GetProductosByQueryAsync(query, (ProductsOrderBy)orden, pag - 1, qty, userId);
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
                var result = await _productoService.GetProductosByCategoriaAsync(categoria, filtros, userId, orden, pag, qty);
                return View("BusquedaProductos", result);
            }
            catch (NotFoundException e)
            {
                Response.StatusCode = 404;
            }
            catch (Exception e)
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

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> EliminarProducto([FromBody] int productoId)
        {
            try
            {
                await _productoService.DeleteProductoAsync(productoId);
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
                await _productoService.FeatureProductAsync(productId, featured);
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


