using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UYCommerce.Data;
using UYCommerce.DTOs;
using UYCommerce.Models;
using UYCommerce.Services;
using UYCommerce.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UYCommerce.Controllers
{
    [Authorize(Policy = "User")]
    public class UsuarioController : Controller
    {
        private readonly ShopContext _context;
        private readonly IEmailService _emailService;

        public UsuarioController(ShopContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }


        [HttpGet]
        [Route("Compras")]
        public async Task<IActionResult> GetCompras()
        {
            List<Orden>? compras = null;

            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            compras = await _context.Ordenes.Where(o => o.UsuarioId == usuarioId)
                .Include(o => o.Productos)!.ThenInclude(p => p.Sku).ThenInclude(s => s!.Imagenes)
                .ToListAsync();

            return View("Compras", compras);
        }

        [HttpGet]
        [Route("Compras/{compraId}")]
        public async Task<IActionResult> BuscarCompraPorId(string compraId)
        {
            var compra = await _context.Ordenes
                .Include(o => o.Direccion)
                .Include(o => o.Productos)!.ThenInclude(p => p.Sku).ThenInclude(s => s!.Imagenes)
                .Include(o => o.Productos)!.ThenInclude(p => p.Sku).ThenInclude(s => s!.AtributosValores)!.ThenInclude(a => a.Atributo)
                .FirstOrDefaultAsync(o => o.Id == compraId);

            if (compra is not null) { return View("VerCompra", compra); }

            return View("NotFound");
        }

        [HttpGet]
        [Route("Favoritos")]
        public async Task<IActionResult> GetFavoritos()
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Favoritos)!
                .Include(u => u.Favoritos)!.ThenInclude(s => s.Imagenes)!
                .FirstOrDefaultAsync(u => u.Id.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (usuario is null) { return View("~/Login"); }

            return View("Favoritos", usuario!.Favoritos);
        }

        [HttpPost]
        public async Task<JsonResult> AgregarPregunta(PreguntaDTO pregunta)
        {

            var producto = await _context.Productos.Include(p => p.Preguntas).FirstOrDefaultAsync(p => p.Id == pregunta.ProductoId);

            if (producto is null || string.IsNullOrEmpty(pregunta.Contenido) || !User.Identity!.IsAuthenticated)
            {

                return new JsonResult("No se pudo agregar la pregunta");
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier));

            Pregunta preguntaNueva = new()
            {
                Contenido = pregunta.Contenido,
                Fecha = DateTime.Now,
                Usuario = usuario!
            };

            producto.Preguntas!.Add(preguntaNueva);

            _context.Update(producto);
            _context.SaveChanges();

            pregunta.NombreUsuario = usuario!.Nombre;
            pregunta.Fecha = DateTime.Now.ToString();

            return new JsonResult(new { pregunta, count = producto.Preguntas.Count });
        }


        [HttpGet]
        [Route("/reviews")]
        public async Task<IActionResult> GetReviews()
        {

            ReviewsVM result = new();

            var usuario = await _context.Usuarios
                .Include(u => u.Reviews)!.ThenInclude(r => r.Producto)!.ThenInclude(p => p!.Imagenes)
                .Include(u => u.Ordenes)!.ThenInclude(o => o.Productos)!.ThenInclude(p => p.Sku).ThenInclude(s => s!.Producto).ThenInclude(p => p!.Imagenes)
                .FirstOrDefaultAsync(u => u.Id == int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!));

            if (usuario is null) return Redirect("/Compras");

            result.Reviews = usuario?.Reviews?.ToList();

            var productosReviews = result.Reviews?.Select(r => r.Producto).ToList();

            var productosOrdenes = usuario!.Ordenes!.SelectMany(o => o.Productos!).Select(p => p.Sku).Select(s => s!.Producto).Distinct();

            var productosPendientesReview = productosOrdenes.Where(p => !productosReviews!.Any(pr => pr!.Id == p!.Id)).ToList();

            result.Productos = productosPendientesReview!;

            return View("Reviews", result);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarReview([FromBody] ReviewDTO reviewDTO)
        {

            if (ModelState.IsValid)
            {

                var producto = await _context.Productos.Include(p => p.Reviews)!.ThenInclude(r => r.Usuario).FirstOrDefaultAsync(p => p.Id == reviewDTO.ProductoId);

                var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                if (producto is null || producto.Reviews!.Any(r => r.UsuarioId == usuarioId))
                {

                    return BadRequest("error");
                }

                Review review = new()
                {
                    ProductoId = reviewDTO.ProductoId,
                    Texto = reviewDTO.Comentario,
                    Puntuacion = reviewDTO.Valoracion,
                    UsuarioId = usuarioId
                };

                producto.Reviews?.Add(review);

                _context.Productos.Update(producto);
                await _context.SaveChangesAsync();

                return Ok("Se agrego la review");
            }

            return BadRequest("Campos invalidos");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateReview([FromBody] UpdateReviewDTO reviewDTO)
        {

            if (ModelState.IsValid)
            {

                var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewDTO.ReviewId);

                if (review is null) return BadRequest();

                review.Texto = reviewDTO.Comentario;
                review.Puntuacion = reviewDTO.Valoracion;

                _context.Update(review);
                await _context.SaveChangesAsync();

                return Ok("Se actualizo la review correctamente");
            }

            return BadRequest("error");
        }
    }
}

