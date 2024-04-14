using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UYCommerce.Data;
using UYCommerce.DTOs;
using UYCommerce.Models;
using UYCommerce.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UYCommerce.Controllers
{
    [Authorize(Policy = "Admin")]
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

        [HttpGet]
        public async Task<IActionResult> Dashboard(DateTime? fchDesde, DateTime? fchHasta)
        {

            fchDesde = fchDesde is null ? DateTime.Now.AddMonths(-4).Date : fchDesde;
            fchHasta = fchHasta is null ? DateTime.Now.Date : fchHasta;

            var ordenes = await _context.Ordenes
                .Include(o => o.Productos)!.ThenInclude(p => p.Sku).ThenInclude(s => s.Imagenes)
                .Where(o => o.FechaDeCompra > fchDesde && o.FechaDeCompra < fchHasta).ToListAsync();

            List<PuntoGrafica> puntos = new();
            List<PuntoGrafica> puntosGanacias = new();

            int i = 0;
            while (fchDesde.Value.AddMonths(i) < fchHasta.Value)
            {
                var fecha = fchDesde.Value.AddMonths(i);

                puntos.Add(new PuntoGrafica
                {
                    x = DateTimeFormatInfo.CurrentInfo.GetMonthName(fecha.Month) + " " + fecha.Year,
                    y = ordenes.Where(o => o.FechaDeCompra.Month == fchDesde.Value.AddMonths(i).Month
                    && o.FechaDeCompra.Year == fecha.Year).SelectMany(o => o.Productos).Count()
                });

                puntosGanacias.Add(new PuntoGrafica
                {
                    x = DateTimeFormatInfo.CurrentInfo.GetMonthName(fecha.Month) + " " + fecha.Year,
                    y = (double)ordenes.Where(o => o.FechaDeCompra.Month == fchDesde.Value.AddMonths(i).Month
                    && o.FechaDeCompra.Year == fecha.Year).Sum(o => o.Total)!
                });

                i += 1;
            }


            DashboardVM result = new()
            {
                FechaDesde = (DateTime)fchDesde,
                FechaHasta = (DateTime)fchHasta,
                Productos = ordenes.SelectMany(o => o.Productos).ToList(),
                PuntosVentas = puntos,
                PuntosGanacias = puntosGanacias,
                Ordenes = ordenes,
            };

            return View(result);
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

        [HttpGet]
        [Route("admin/clientes")]
        public async Task<IActionResult> GetClientes()
        {

            var clientes = await _context.Usuarios
                .Include(u => u.Carrito).ThenInclude(c => c.Productos)
                .Include(u => u.Preguntas)
                .Include(u => u.Ordenes)
                .Include(u => u.Reviews)
                .Where(u => u.Rol!.Nombre == "User")
                .ToListAsync();

            return View(clientes);
        }

        [HttpGet]
        [Route("admin/productos")]
        public async Task<IActionResult> GetProductos()
        {

            var result = await _context.Productos
                .Include(p => p.Imagenes)
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.Preguntas)
                .Include(p => p.Reviews)
                .Include(p => p.Skus)!.ThenInclude(s => s.Imagenes)
                .Include(p => p.Skus)!.ThenInclude(s => s.AtributosValores)!.ThenInclude(a => a.Atributo)
                .ToListAsync();
            return View(result);
        }

        [HttpGet]
        [Route("admin/atributos")]
        public async Task<IActionResult> GetAtributos()
        {

            var result = await _context.Atributos
                .Include(a => a.Categorias)
                .Include(a => a.Productos)
                .Include(a => a.Valores)
                .ToListAsync();

            return View(result);
        }


        [HttpGet]
        [Route("admin/ordenes")]
        public async Task<IActionResult> GetOrdenes()
        {

            var ordenes = await _context.Ordenes
                .Include(o => o.Productos)!.ThenInclude(p => p.Sku)
                .Include(o => o.Direccion)
                .ToListAsync();

            List<OrdenVM> result = ordenes.Join(_context.Usuarios, o => o.UsuarioId, u => u.Id, (o, u) => new OrdenVM { Orden = o, Usuario = u }).ToList();


            return View(result);
        }

        [HttpGet]
        [Route("admin/reviews")]
        public async Task<IActionResult> GetReviews()
        {

            var result = await _context.Reviews
                .Include(r => r.Usuario)
                .Include(r => r.Producto).ThenInclude(p => p!.Imagenes)
                .ToListAsync();

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarReview([FromBody] int reviewId) {

            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review is null) return BadRequest("no existe esa review");

            _context.Remove(review);
            await _context.SaveChangesAsync();

            return Ok("Se elimino exitosamente");
        }

        [HttpPost]
        public async Task<IActionResult> AgregarRespuesta([FromBody] RespuestaDTO respuesta)
        {

            var pregunta = await _context.Preguntas.Include(p => p.Respuesta).FirstOrDefaultAsync(p => p.Id == respuesta.PreguntaId);

            if (pregunta is null || string.IsNullOrEmpty(respuesta.Contenido) || pregunta.Respuesta is not null)
                return new BadRequestObjectResult("No se pudo adjuntar la respuesta");

            Respuesta respuestaNueva = new()
            {
                Contenido = respuesta.Contenido,
                Fecha = DateTime.Now,
                PreguntaId = pregunta.Id
            };

            pregunta.Respuesta = respuestaNueva;

            _context.Update(pregunta);
            await _context.SaveChangesAsync();

            return Ok(new { respuestaNueva.Contenido, respuestaNueva.PreguntaId, respuestaNueva.Fecha});

        }
    }
}

