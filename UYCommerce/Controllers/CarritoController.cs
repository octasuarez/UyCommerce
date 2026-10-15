using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UyCommerce.Domain.Entities;
using UyCommerce.Domain.Exceptions;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using System.Security.Claims;

namespace UYCommerce.Controllers
{
    [Authorize(Policy = "User")]
    public class CarritoController : Controller
    {
        private readonly ICarritoService _carritoService;

        public CarritoController(ICarritoService carritoService)
        {
            this._carritoService = carritoService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> AgregarProductoAlCarrito(AddToCartModel request)
        {
            if (!int.TryParse(request.skuId, out int skuId) 
                || !int.TryParse(request.quantity, out int quantity))
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json(new { success = false, error = "invalid numbers" });
            }
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var qty = await _carritoService.AddProductoAlCarritoAsync(userId, skuId, quantity);
                HttpContext.Session.SetString("cartItems", qty.ToString());
                return new JsonResult(new { qty });
            }
            catch (NotFoundException e)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return Json(new { success = false, error = e.Message });
            }
            catch (ArgumentOutOfRangeException e)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json(new { success = false, error = e.Message });
            }
            catch (Exception e)
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Json(new { success = false, error = e.Message });
            }
        }

        [HttpPost]
        public async Task EliminarProductoCarrito(string skuId)
        {
            if (!int.TryParse(skuId, out int sku))
                Response.StatusCode = StatusCodes.Status400BadRequest;
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _carritoService.DeleteProductoCarritoAsync(userId, sku);
                var carritoItemsCount = HttpContext.Session.GetInt32("cartItems") -1 ?? 0;
                HttpContext.Session.SetString("cartItems", carritoItemsCount.ToString());
            }
            catch (NotFoundException e)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
            }
            catch(Exception e)
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }
    }
}

