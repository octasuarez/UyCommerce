using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UYCommerce.Data;
using UYCommerce.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UYCommerce.Controllers
{
    public class MarcaController : Controller
    {
        private readonly ShopContext _context;

        public MarcaController(ShopContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetMarcas()
        {
            return Ok(await _context.Marcas.ToListAsync());
        }
    }
}

