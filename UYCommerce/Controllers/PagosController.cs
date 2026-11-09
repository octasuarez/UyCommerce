using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Domain.Exceptions;

namespace UYCommerce.Controllers
{
    [Authorize(Policy = "User")]
    public class PagosController : Controller
    {
        private readonly ICarritoService _carritoService;
        private readonly IPagosService _pagosService;

        public PagosController(ICarritoService carritoService, IPagosService pagosService)
        {
            this._carritoService = carritoService;
            this._pagosService = pagosService;
        }

        public IActionResult Success()
        {
            return View();
        }

        [HttpGet]
        [Route("Checkout")]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var carrito = await _carritoService.GetCarritoCompleteByUserId(userId);
                return View(carrito);
            }
            catch (NotFoundException) { }
            catch (Exception) { }
            return RedirectToAction("/");
        }

        #region paypal

        [HttpPost]
        [Route("/create-paypal-order")]
        public async Task<IActionResult> CreateOrder()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Ok(await _pagosService.CreateOrderPaypal(userId));
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        [Route("/orders/{orderID}/capture")]
        public async Task<IActionResult> CaptureOrder(string? orderID)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                return Ok(await _pagosService.CaptureOrderPaypal(orderID,userId));
            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }
        }

        #endregion paypal
    }
}

