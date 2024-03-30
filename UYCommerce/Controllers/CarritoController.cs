using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UYCommerce.Data;
using UYCommerce.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UYCommerce.Controllers
{
    [Authorize(Policy = "User")]
    public class CarritoController : Controller
    {


        private readonly ShopContext _context;

        public CarritoController(ShopContext context)
        {

            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Agrega un producto al carrito si no existe. Aumenta o disminuye la cantidad del producto si ya existe.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AgregarProductoAlCarrito(AddToCartModel request)
        {
            Carrito? carrito = GetCarrito();

            if (carrito is not null)
            {
                var sku = _context.Skus.FirstOrDefault(s => s.Id == int.Parse(request.skuId));

                var productoCarrito = carrito.Productos!.FirstOrDefault(p => p.SkuId == int.Parse(request.skuId));
                int qty = 0;
                var cantidad = int.Parse(request.quantity);

                if (productoCarrito is null && cantidad > 0 && cantidad <= sku?.Stock)
                {
                    ProductoCarrito nuevoProductoCarrito = new() { Cantidad = cantidad, SkuId = int.Parse(request.skuId) };
                    carrito.Productos!.Add(nuevoProductoCarrito);
                }
                else if (productoCarrito is not null && (cantidad + productoCarrito.Cantidad) <= sku?.Stock)
                {
                    carrito.Productos!.FirstOrDefault(p => p.Equals(productoCarrito))!.Cantidad += cantidad;
                    qty = carrito.Productos!.FirstOrDefault(p => p.Equals(productoCarrito))!.Cantidad;
                    if (qty == 0)
                        carrito.Productos!.Remove(carrito.Productos!.FirstOrDefault(p => p.SkuId == productoCarrito.SkuId)!);
                }
                else
                {
                    return new JsonResult(new EmptyResult());
                }

                _context.Update(carrito);
                _context.SaveChanges();
                HttpContext.Session.SetString("cartItems", carrito.Productos!.Count.ToString());

                return new JsonResult(new { qty });
            }

            return new JsonResult(new EmptyResult());

        }

        /// <summary>
        /// Elimina un producto del carrito.
        /// </summary>
        /// <param name="skuId"></param>
        [HttpPost]
        public void EliminarProductoCarrito(string skuId)
        {

            Carrito? carrito = GetCarrito();

            if (carrito != null && carrito.Productos != null)
            {
                var productoCarrito = carrito.Productos.FirstOrDefault(p => p.SkuId == int.Parse(skuId));

                if (productoCarrito != null)
                {
                    carrito.Productos.Remove(productoCarrito);
                    _context.Update(carrito);
                    _context.SaveChanges();
                    HttpContext.Session.SetString("cartItems", carrito.Productos!.Count.ToString());

                }
            }
        }

        /// <summary>
        /// Retorna el carrito del usuario sea que este logueado o no.
        /// </summary>
        /// <returns></returns>
        private Carrito? GetCarrito()
        {
            var carrito = new Carrito() { Productos = new List<ProductoCarrito>() };

            carrito = _context.Carritos
                .Include(c => c.Productos)!.ThenInclude(p => p.Sku).ThenInclude(s => s.Imagenes)
                .FirstOrDefault(c => c.UsuarioId == int.Parse(User.FindFirst("CarritoId")!.Value));

            return carrito;
        }

    }
}

