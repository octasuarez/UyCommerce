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

                //Pasamos los productos y el total de la compra para poder crear la orden
                //El total en double lo dividimos en 40 para pasarlo a USD y lo formateamos para tener solo 2 digitos despues de la coma.

                //var total = String.Format("{0:0.00}", (carrito!.GetTotal() / 40));

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

                if (response is not null && User.Identity.IsAuthenticated)
                {

                    var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!));

                    Orden orden = new()
                    {
                        Id = response.id!,
                        UsuarioId = usuario != null ? usuario.Id : null,
                        FechaDeCompra = DateTime.Now,
                        Estado = "en camino",
                        NombreComprador = usuario != null ? usuario.Nombre : null,
                    };


                    //Compra compra = new()
                    //{
                    //    CompraId = response.id!,
                    //    UsuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                    //    FechaDeCompra = DateTime.Now.ToShortDateString(),
                    //};

                    foreach (var item in response.purchase_units![0].items!)
                    {

                        orden.Productos?.Add(new ProductoOrden
                        {

                            SkuId = int.Parse(item.sku!),
                            OrdenId = orden.Id,
                            Cantidad = int.Parse(item.quantity!),
                            Precio = double.Parse(item.unit_amount!.value!) * 40, //a dolares
                            Nombre = item.name,
                        });
                    }

                    //var carritoId = User.FindFirstValue("CarritoID");

                    //await _context.ProductosCarritos.Where(p => p.CarritoId.ToString() == carritoId).ExecuteDeleteAsync();

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

    }
}

