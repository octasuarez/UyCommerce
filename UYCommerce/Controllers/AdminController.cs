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
    public class AdminController : Controller
    {
        private readonly ShopContext _context;

        public AdminController(ShopContext context)
        {

            _context = context;
        }


        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }



        public async Task<IActionResult> Dashboard()
        {


            return View();
        }

        [HttpGet("admin/preguntas/{orden?}")]
        public async Task<IActionResult> GetPreguntas(string? orden)
        {


            IQueryable<Pregunta> preguntas = _context.Preguntas
                .Include(p => p.Usuario)
                .Include(p => p.Producto).ThenInclude(pr => pr.Imagenes)
                .Include(p => p.Respuesta);

            if (orden == "sin responder")
            {
                preguntas = preguntas.Where(p => p.Respuesta == null);
            }

            if (orden == "respondidas")
            {
                preguntas = preguntas.Where(p => p.Respuesta != null);
            }

            ViewBag.selected = orden;

            var resultado = await preguntas.ToListAsync();

            return View("Preguntas", resultado);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarRespuesta(int preguntaId, string respuesta)
        {

            var pregunta = await _context.Preguntas.FirstOrDefaultAsync(p => p.Id == preguntaId);

            if (pregunta is null || string.IsNullOrEmpty(respuesta))
            {

                return new BadRequestObjectResult("No se pudo adjuntar la respuesta");
            }

            Respuesta respuestaNueva = new()
            {

                Contenido = respuesta,
                Fecha = DateTime.Now,
                PreguntaId = pregunta.Id

            };

            pregunta.Respuesta = respuestaNueva;

            _context.Update(pregunta);
            await _context.SaveChangesAsync();

            return Json(respuesta);

        }
    }
}

