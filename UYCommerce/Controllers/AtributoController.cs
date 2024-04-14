using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UYCommerce.Data;
using UYCommerce.DTOs;
using UYCommerce.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UYCommerce.Controllers
{
    public class AtributoController : Controller
    {
        private readonly ShopContext _context;

        public AtributoController(ShopContext context)
        {

            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CrearAtributo()
        {

            return View();
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> CrearAtributo([FromBody] string nombre)
        {

            if (string.IsNullOrEmpty(nombre))
                return BadRequest("Escriba un nombre");

            var atributo = await _context.Atributos.FirstOrDefaultAsync(a => a.Nombre!.ToLower() == nombre.ToLower());

            if (atributo is not null)
                return BadRequest("Ya existe un atributo con ese nombre");

            Atributo nuevoAtributo = new() { Nombre = nombre };

            await _context.AddAsync(nuevoAtributo);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AgregarValor([FromBody] AgregarValorDTO atributoValor)
        {

            if (string.IsNullOrEmpty(atributoValor.Valor))
                return BadRequest(new { status = 400, error = "escriba un valor" });

            var atributo = await _context.Atributos
                .Include(a => a.Valores)
                .FirstOrDefaultAsync(a => a.Id == atributoValor.atributoId);

            if (atributo is null)
                return BadRequest(new { status = 400, error = "Error, no se econtro el atributo" });

            var valorExiste = atributo.Valores?.FirstOrDefault(v => v.Valor?.ToLower() == atributoValor.Valor.ToLower());

            if (valorExiste is not null)
                return BadRequest(new { status = 400, error = "Ese valor ya existe" });

            atributo.Valores?.Add(new AtributoValor { Valor = atributoValor.Valor });

            _context.Update(atributo);
            await _context.SaveChangesAsync();

            return Ok(new { status = 200 });
        }


        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetAtributosJson()
        {
            return Ok(await _context.Atributos.ToListAsync());
        }
    }
}

