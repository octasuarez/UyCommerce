using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Application.DTOs;
using UyCommerce.Domain.Entities;
using UyCommerce.Domain.Exceptions;
using UYCommerce.ViewModels;


namespace UYCommerce.Controllers
{
    [Authorize(Policy = "Admin")]
    public class AdminController : Controller
    {
        private readonly IOrdenService _ordenService;
        private readonly IPreguntaService _preguntaService;
        private readonly IUsuarioService _usuarioService;
        private readonly IProductoService _productoService;
        private readonly IReviewService _reviewService;
        private readonly IAtributoService _atributoService;
        private readonly IImagesService _imagesService;

        public AdminController(
             IOrdenService ordenService
            , IPreguntaService preguntaService
            , IUsuarioService usuarioService
            , IProductoService productoService
            , IReviewService reviewService
            , IAtributoService atributoService
            , IImagesService imagesService)
        {
            _ordenService = ordenService;
            _preguntaService = preguntaService;
            _usuarioService = usuarioService;
            _productoService = productoService;
            _reviewService = reviewService;
            _atributoService = atributoService;
            _imagesService = imagesService;

        }

        public IActionResult Index()
        {
            return View();
        }

        #region [HttpGet]

        [HttpGet]
        public async Task<IActionResult> Dashboard(DateTime? fchDesde, DateTime? fchHasta)
        {
            try
            {
                fchDesde ??= DateTime.Now.AddYears(-1);
                fchHasta ??= DateTime.Now;
                return View(await _ordenService.GetDashboard(fchDesde, fchHasta));
            }
            catch (Exception)
            {
            }
            return View();
        }

        [HttpGet("admin/preguntas/{option?}")]
        public async Task<IActionResult> GetPreguntas(string? option)
        {
            try
            {
                var answered = option?.Equals("respondidas");
                var preguntas = await _preguntaService.GetPreguntas(answered);
                ViewBag.selected = option;

                return View("Preguntas", preguntas);
            }
            catch (Exception e) { }
            return View("Preguntas");
        }

        [HttpGet]
        [Route("admin/clientes")]
        public async Task<IActionResult> GetClientes()
        {
            try
            {
                return View(await _usuarioService.GetUsuariosClientes());
            }
            catch (Exception e)
            { }
            return View();
        }

        [HttpGet]
        [Route("admin/productos")]
        public async Task<IActionResult> GetProductos()
        {
            try
            {
                return View(await _productoService.GetProductosComplete());
            }
            catch (Exception e)
            { }
            return View();
        }

        [HttpGet]
        [Route("admin/atributos")]
        public async Task<IActionResult> GetAtributos()
        {
            try
            {
                return View(await _atributoService.GetAtributos());
            }
            catch (Exception e)
            { }
            return View();
        }


        [HttpGet]
        [Route("admin/ordenes")]
        public async Task<IActionResult> GetOrdenes()
        {
            try
            {
                return View(await _ordenService.GetOrdenesUsuarios());
            }
            catch (Exception e)
            { }
            return View();
        }

        [HttpGet]
        [Route("admin/reviews")]
        public async Task<IActionResult> GetReviews()
        {
            try
            {
                return View(await _reviewService.GetReviews());
            }
            catch (Exception e) { }
            return View();
        }

        [HttpGet]
        [Route("admin/carousel")]
        public async Task<IActionResult> Carousel()
        {
            try
            {
                return View(await _imagesService.GetCarouselImages());
            }
            catch (Exception e)
            { }
            return View();
        }
        #endregion [HttpGet]

        #region [HttpPost]

        [HttpPost]
        public async Task<IActionResult> DeleteCarouselImage(int imageId)
        {
            try
            {
                await _imagesService.DeleteCarouselImage(imageId);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

            return Ok("Imagen borrada existosamente");
        }

        [HttpPost]
        public async Task<IActionResult> EliminarReview([FromBody] int reviewId)
        {
            try
            {
                await _reviewService.DeleteReview(reviewId);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }

            return Ok("Review eliminada exitosamente");
        }
        [HttpPost]
        public async Task<IActionResult> AgregarRespuesta([FromBody] RespuestaDTO respuesta)
        {
            try
            {
                await _preguntaService.UpdatePreguntaRespuesta(respuesta);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok(new { respuesta.Contenido, respuesta.PreguntaId, DateTime.Now });

        }

        [HttpPost]
        public async Task<IActionResult> CreateCarouselImage(IFormFile file)
        {
            if (file is null || file.Length <= 0) return BadRequest("Imagen invalida");
            try
            {
                using var fileStream = file.OpenReadStream();
                await _imagesService.AddCarouselImage(file.Name, fileStream);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeCarouselImage(IFormFile file, int imageId)
        {
            if (file is null || file.Length <= 0) return BadRequest("Imagen invalida");
            try
            {
                var stream = file.OpenReadStream();
                await _imagesService.UpdateCarouselImage(imageId, file.Name, stream);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok();
        }

        #endregion [HttpPost]
    }
}

