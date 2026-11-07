using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UyCommerce.Application.DTOs;
using UyCommerce.Domain.Entities;
using UyCommerce.Application.Contracts.Persistance.Services;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Domain.Exceptions;

namespace UYCommerce.Controllers
{
    [Authorize(Policy = "User")]
    public class UsuarioController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly IPreguntaService _preguntaService;
        private readonly IUsuarioService _usuarioService;
        private readonly IOrdenService _ordenService;

        public UsuarioController(IEmailService emailService
            , IPreguntaService preguntaService
            , IUsuarioService usuarioService
            , IOrdenService ordenService)
        {
            this._emailService = emailService;
            this._preguntaService = preguntaService;
            this._usuarioService = usuarioService;
            this._ordenService = ordenService;
        }

        #region ordenes

        [HttpGet]
        [Route("Compras")]
        public async Task<IActionResult> GetCompras()
        {
            List<Orden> ordenes = [];
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                ordenes = await _ordenService.GetOrdenesByUsuarioId(userId);
            }
            catch (Exception e)
            {
            }
            return View("Compras", ordenes);
        }

        [HttpGet]
        [Route("Compras/{compraId}")]
        public async Task<IActionResult> BuscarCompraPorId(int compraId)
        {
            try
            {
                return View("VerCompra", await _ordenService.GetOrdenById(compraId));
            }
            catch (NotFoundException e)
            {
                Response.StatusCode = 404;
            }
            catch (Exception e)
            {
            }
            return View("NotFound");
        }

        #endregion ordenes

        #region favoritos

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
        public async Task<IActionResult> AgregarAFavoritos(int skuId)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Ok(await _usuarioService.AddToFavoritos(skuId, userId));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        #endregion favoritos

        #region reviews

        [HttpGet]
        [Route("/reviews")]
        public async Task<IActionResult> GetReviews()
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await _usuarioService.GetReviewsAndProductos(userId);
                return View("Reviews", result);
            }
            catch (Exception e)
            {
            }
            return Redirect("/Compras");
        }

        [HttpPost]
        public async Task<IActionResult> AgregarReview([FromBody] ReviewDTO reviewDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    await _usuarioService.CreateReview(reviewDTO, userId);
                    return Ok("Se agrego la review");
                }
                catch (NotFoundException)
                {
                    return BadRequest();
                }
                catch (Exception e)
                {
                    return StatusCode(500);
                }
            }
            return BadRequest("Campos invalidos");

        }

        [HttpPost]
        public async Task<IActionResult> UpdateReview([FromBody] UpdateReviewDTO reviewDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    await _usuarioService.UpdateReview(reviewDTO, userId);
                    return Ok("Se actualizo la review correctamente");
                }
                catch (Exception e)
                {
                }
            }
            return BadRequest();
        }

        #endregion reviews

        #region preguntas

        [HttpPost]
        public async Task<IActionResult> AgregarPregunta(CreatePreguntaDTO pregunta)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                pregunta.Fecha = DateTime.Now;
                await _preguntaService.CreatePregunta(pregunta, userId);
                var nombreUsuario = User.FindFirstValue("FullName") ?? "";
                return Ok(new PreguntaDto { Contenido = pregunta.Contenido, Fecha = pregunta.Fecha.ToString("dd/MM/yyyy HH:mm:ss"), NombreUsuario = nombreUsuario });
            }
            catch (Exception)
            {
                return BadRequest("No se pudo agregar la pregunta");
            }
        }

        #endregion preguntas
    }
}

