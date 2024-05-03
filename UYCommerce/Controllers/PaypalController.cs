using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UYCommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using UYCommerce.Paypal;
using UYCommerce.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LoginAndRegister.Controllers
{
    public class PaypalController : Controller
    {
        private readonly PaypalAPI _paypalAPI;
        private readonly ShopContext _context;

        public PaypalController(PaypalAPI PaypalAPI, ShopContext context)
        {

            this._paypalAPI = PaypalAPI;
            this._context = context;

        }

        //Crea una orden en Paypal
        [HttpPost]
        [Route("/create-paypal-order")]
        public async Task<IActionResult> CreateOrder()
        {
            try
            {
                Carrito? carrito = new();

                if (User.Identity is not null && User.Identity.IsAuthenticated)
                {

                    var usuarioID = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

                    carrito = _context.Carritos
                        .Include(c => c.Productos)!
                        .ThenInclude(pC => pC.Sku)
                        .FirstOrDefault(c => c.UsuarioId.ToString() == usuarioID);
                }

                var total = carrito!.GetTotal().ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);

                var response = await _paypalAPI.CreateOrder(carrito.Productos, total.ToString());


                return Ok(response);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        //Captura la orden del cliente
        [HttpPost]
        [Route("/orders/{orderID}/capture")]
        public async Task<IActionResult> CaptureOrder(string? orderID)
        {
            try
            {
                var response = await _paypalAPI.CaptureOrder(orderID);

                if (response is not null && User.Identity!.IsAuthenticated)
                {
                    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                    var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == userId)
                        ?? throw new Exception("The user was not found");

                    Orden orden = CreateOrder(response, usuario);

                    await UpdateStock(orden.Productos!);

                    await DeleteCartProducts();

                    await _context.AddAsync(orden);
                    await _context.SaveChangesAsync();

                }

                return Ok(response);
            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }
        }

        private async Task UpdateStock(ICollection<ProductoOrden> products)
        {

            var skusIds = products.Select(p => p.SkuId);

            var skus = await _context.Skus.Include(s => s.Producto)
                .Where(s => skusIds.Any(skuId => skuId == s.Id)).ToListAsync();

            foreach (var s in skus)
            {
                s.Stock -= products.First(p => p.SkuId == s.Id).Cantidad;
                s.Producto!.VecesComprado += 1;
            }

            _context.UpdateRange(skus);
        }

        private static Orden CreateOrder(CaptureOrderResponse response, Usuario usuario)
        {

            Orden orden = new()
            {
                Id = response.id!,
                UsuarioId = usuario?.Id,
                FechaDeCompra = DateTime.Now,
                Estado = "en camino",
                NombreComprador = usuario?.Nombre,
                MetodoDePago = response.payment_source!.card != null ? "Tarjeta" : "Paypal",
                Total = double.Parse(response.purchase_units![0].amount!.value!),
                Direccion = JsonSerializer.Deserialize<OrdenDireccion>(JsonSerializer.Serialize(response.purchase_units[0].shipping?.address)),
                Productos = new List<ProductoOrden>()
            };

            foreach (var item in response.purchase_units![0].items!)
            {
                orden.Productos?.Add(new ProductoOrden
                {
                    SkuId = int.Parse(item.sku!),
                    OrdenId = orden.Id,
                    Cantidad = int.Parse(item.quantity!),
                    Precio = double.Parse(item.unit_amount!.value!),
                    Nombre = item.name,
                });
            }

            return orden;
        }

        private async Task DeleteCartProducts() {

            var cartId = int.Parse(User.FindFirstValue("CarritoId")!);
            await _context.ProductosCarritos.Where(p => p.CarritoId == cartId).ExecuteDeleteAsync();
            HttpContext.Session.SetString("cartItems", "0");

        }

    }
}

