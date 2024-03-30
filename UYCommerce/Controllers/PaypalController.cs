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

                var total = carrito!.GetTotal();

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

                    var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!));

                    Orden orden = new()
                    {
                        Id = response.id!,
                        UsuarioId = usuario?.Id,
                        FechaDeCompra = DateTime.Now,
                        Estado = "en camino",
                        NombreComprador = usuario?.Nombre,
                        MetodoDePago = response.payment_source!.card != null ? "Tarjeta" : "Paypal",
                        Total = double.Parse(response.purchase_units![0].amount!.value!),
                        Direccion = JsonSerializer.Deserialize<OrdenDireccion>(JsonSerializer.Serialize(response.purchase_units[0].shipping.address)),
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

                    var carrito = await _context.Carritos
                        .Include(c => c.Productos)!
                        .ThenInclude(p => p.Sku).ThenInclude(s => s!.Producto)
                        .FirstOrDefaultAsync(c => c.Id.ToString() == User.FindFirstValue("CarritoId"));

                    carrito?.Productos?.Select(p => p.Sku).Select(s => s!.Producto).ToList().ForEach(p => p!.VecesComprado += 1);
                    carrito?.Productos?.ToList().ForEach(p => p.Sku!.Stock -= p.Cantidad);

                    await _context.ProductosCarritos.Where(p => p.CarritoId == carrito!.Id).ExecuteDeleteAsync();

                    await _context.AddAsync(orden);
                    _context.Carritos.Update(carrito!);
                    await _context.SaveChangesAsync();

                    HttpContext.Session.SetString("cartItems", "0");
                }

                return Ok(response);
            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }
        }

    }
}

