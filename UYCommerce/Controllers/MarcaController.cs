using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        [Route("admin/marcas")]
        public async Task<IActionResult> GetMarcas()
        {
            var marcas = await _context.Marcas
                .Include(m => m.Productos)
                .ToListAsync();
            return View("../Admin/GetMarcas", marcas);
        }

        [HttpGet]
        public async Task<IActionResult> GetMarcasApi()
        {
            return Ok(await _context.Marcas.ToListAsync());
        }

        [HttpGet]
        public IActionResult CreateMarca()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> CreateMarca(string nombre)
        {

            var error = await MarcaIsValid(nombre);
            if (error is not null) {
                return BadRequest(new { error });
            }

            _context.Marcas.Add(new Marca { Nombre = nombre });
            await _context.SaveChangesAsync();

            return Ok();
        }

        private async Task<string?> MarcaIsValid(string nombre)
        {

            if (string.IsNullOrEmpty(nombre))
            {
                return "Campo obligatorio";
            }

            var marca = await _context.Marcas.FirstOrDefaultAsync(m => m.Nombre!.ToLower() == nombre.ToLower());

            if (marca is not null)
            {
                return "Marca ya existe";
            }

            return null;
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> DeleteMarca(int marcaId) {


            var marca = await GetMarcaById(marcaId);

            if(marca is null) {
                return BadRequest("Marca no econtrada");
            }

            _context.Remove(marca);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        public async Task<Marca?> GetMarcaById(int id) {

            var marca = await _context.Marcas
                .Include(m => m.Productos)
                .FirstOrDefaultAsync(m => m.Id == id);

            return marca;
        }
    }
}

