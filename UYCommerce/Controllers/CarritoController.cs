using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UYCommerce.Data;
using UYCommerce.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UYCommerce.Controllers
{
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


        [HttpPost]
        public async void AgregarProductoAlCarrito(AddToCartModel request)
        {

            Carrito? carrito = GetCarrito();

            var sku = _context.Skus.FirstOrDefault(s => s.Id == int.Parse(request.skuId));

            var productoCarrito = carrito.Productos!.FirstOrDefault(p => p.SkuId == int.Parse(request.skuId));

            if (productoCarrito is null)
            {
                ProductoCarrito nuevoProductoCarrito = new() { Cantidad = 1, SkuId = int.Parse(request.skuId) };
                carrito.Productos.Add(nuevoProductoCarrito);
            }
            else
            {
                carrito.Productos.FirstOrDefault(p => p.SkuId == int.Parse(request.skuId))!.Cantidad++;
            }

            if (User.Identity.IsAuthenticated)
            {
                _context.Update(carrito);
                _context.SaveChanges();
            }
            else
            {

                var sessionCarrito = JsonSerializer.Serialize(carrito);

                HttpContext.Session.SetString("cart", sessionCarrito);
            }

        }

        /// <summary>
        /// Retorna el carrito del usuario sea que este logueado o no
        /// </summary>
        /// <returns></returns>
        private Carrito? GetCarrito()
        {

            var carrito = new Carrito() { Productos = new List<ProductoCarrito>()};

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                carrito = _context.Carritos
                    .Include(c => c.Productos)!.ThenInclude(p=> p.Sku).ThenInclude(s=>s.Imagenes)
                    .FirstOrDefault(c => c.UsuarioId == int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value));
            }
            else
            {
                if (HttpContext.Session.GetString("cart") != null) 
                {
                    carrito = JsonSerializer.Deserialize<Carrito>(HttpContext.Session.GetString("cart"));
                }
                else
                {
                    var sessionCart = JsonSerializer.Serialize(carrito);

                    HttpContext.Session.SetString("cart", sessionCart);
                }

            }

            return carrito;

        }

        [HttpGet]
        public async Task<IActionResult> DetallesCarrito()
        {


            return View();
        }
    }
}

