using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UYCommerce.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UYCommerce.Controllers
{
    public class PagosController : Controller
    {
        private readonly ShopContext _context;

        public PagosController(ShopContext context)
        {

            _context = context;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("Checkout")]
        public IActionResult Checkout()
        {

            var usuarioID = User.FindFirst(ClaimTypes.NameIdentifier);

            if (usuarioID == null)
            {
                return Redirect("Login");
            }

            var carrito = _context.Carritos
                .Include(c => c.Productos)!.ThenInclude(p => p.Sku).ThenInclude(s => s.Imagenes)
                .Include(c => c.Productos)!.ThenInclude(p => p.Sku).ThenInclude(s => s.Producto).ThenInclude(pr => pr.Marca)
                .Include(c => c.Productos)!.ThenInclude(p => p.Sku).ThenInclude(s => s.AtributosValores)!.ThenInclude(a => a.Atributo)
                .FirstOrDefault(c => c.UsuarioId.ToString() == usuarioID.Value);


            return View(carrito);
        }


        public IActionResult Success()
        {

            return View();
        }
    }
}

