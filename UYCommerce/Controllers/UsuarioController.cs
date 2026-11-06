using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UyCommerce.Infrastructure.Persistance;
using UyCommerce.Application.DTOs;
using UyCommerce.Domain.Entities;
using UYCommerce.ViewModels;
using UyCommerce.Application.Contracts.Persistance.Services;
using UyCommerce.Application.Contracts.ServicesAbstractions;

namespace UYCommerce.Controllers
{
    [Authorize(Policy = "User")]
    public class UsuarioController : Controller
    {
        private readonly ShopContext _context;
        private readonly IEmailService _emailService;
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(ShopContext context, IEmailService emailService, IUsuarioService usuarioService)
        {
            _context = context;
            _emailService = emailService;
            this._usuarioService = usuarioService;
        }

        [HttpGet]
        [Route("Compras")]
        public async Task<IActionResult> GetCompras()
        {
            List<Orden>? compras = null;

            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            compras = await _context.Ordenes.Where(o => o.Usuario.Id == usuarioId)
                .Include(o => o.Productos)!.ThenInclude(p => p.Sku).ThenInclude(s => s!.Imagenes)
                .ToListAsync();

            return View("Compras", compras);
        }

        [HttpGet]
        [Route("Compras/{compraId}")]
        public async Task<IActionResult> BuscarCompraPorId(int compraId)
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
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                return View("Favoritos", await _usuarioService.GetFavoritosByUsuarioId(userId));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AgregarPregunta(CreatePreguntaDTO pregunta)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                pregunta.Fecha = DateTime.Now;
                await _usuarioService.CreatePregunta(pregunta, userId);
                var nombreUsuario = User.FindFirstValue("FullName") ?? "";
                return Ok(new PreguntaDto { Contenido = pregunta.Contenido, Fecha = pregunta.Fecha.ToString("dd/MM/yyyy HH:mm:ss"), NombreUsuario = nombreUsuario});
            }
            catch (Exception)
            {
                return BadRequest("No se pudo agregar la pregunta");
            }
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

        [HttpPost]
        public async Task<IActionResult> AgregarAFavoritos(string skuId)
        {
            var sku = await _context.Skus.FirstOrDefaultAsync(s => s.Id.ToString() == skuId);
            if (sku is null)
            {
                return new BadRequestObjectResult("El producto no fue encontrado");
            }
            var usuario = await _context.Usuarios.Include(u => u.Favoritos)
                .FirstOrDefaultAsync(u => u.Id.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier));
            string msj = "";
            if (usuario is null)
            {
                return new BadRequestObjectResult("El producto no fue encontrado");
            }
            else
            {
                if (usuario.Favoritos!.Contains(sku))
                {
                    usuario.Favoritos.Remove(sku);
                    msj = "eliminado";
                }
                else
                {
                    usuario.Favoritos.Add(sku);
                    msj = "agregado";

                }

            }

            _context.Update(usuario);
            await _context.SaveChangesAsync();

            return Json(new { msj });

        }
    }
}

