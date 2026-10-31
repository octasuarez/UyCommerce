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
            fchDesde ??= DateTime.Now.AddYears(-1);
            fchHasta ??= DateTime.Now;
            List<PuntoGrafica> puntos = [];
            List<PuntoGrafica> puntosGanacias = [];
            List<ProductoOrden> productos = [];
            var ordenes = await _ordenService.GetOrdenesByDateAsync(fchDesde, fchHasta);

            if (ordenes != null && ordenes.Count > 0)
            {
                productos = [.. ordenes.Where(o => o.Productos != null).SelectMany(o => o.Productos)];

                int i = 0;
                DateTime fecha = fchDesde.Value;
                while (fecha < fchHasta.Value)
                {
                    fecha = fchDesde.Value.AddMonths(i);

                    puntos.Add(new PuntoGrafica
                    {
                        x = DateTimeFormatInfo.CurrentInfo.GetMonthName(fecha.Month) + " " + fecha.Year,
                        y = ordenes.Where(o => o.FechaDeCompra.Month == fecha.Month
                        && o.FechaDeCompra.Year == fecha.Year)
                        .Where(o => o.Productos != null).SelectMany(o => o.Productos).Count()
                    });

                    puntosGanacias.Add(new PuntoGrafica
                    {
                        x = DateTimeFormatInfo.CurrentInfo.GetMonthName(fecha.Month) + " " + fecha.Year,
                        y = (double)ordenes.Where(o => o.FechaDeCompra.Month == fecha.Month
                        && o.FechaDeCompra.Year == fecha.Year).Sum(o => o.Total)!
                    });

                    i += 1;
                }
            }
            DashboardVM result = new()
            {
                FechaDesde = (DateTime)fchDesde,
                FechaHasta = (DateTime)fchHasta,
                Productos = productos,
                PuntosVentas = puntos,
                PuntosGanacias = puntosGanacias,
                Ordenes = ordenes ?? [],
            };

            return View(result);
        }

        [HttpGet("admin/preguntas/{option?}")]
        public async Task<IActionResult> GetPreguntas(string? option)
        {
            var answered = option?.Equals("respondidas");
            var preguntas = await _preguntaService.GetPreguntas(answered);
            ViewBag.selected = option;

            return View("Preguntas", preguntas);
        }

        [HttpGet]
        [Route("admin/clientes")]
        public async Task<IActionResult> GetClientes()
        {
            var clientes = await _usuarioService.GetUsuariosClientesAsync();
            return View(clientes);
        }

        [HttpGet]
        [Route("admin/productos")]
        public async Task<IActionResult> GetProductos()
        {

            var productos = await _productoService.GetProductosCompleteAsync();
            return View(productos);
        }

        [HttpGet]
        [Route("admin/atributos")]
        public async Task<IActionResult> GetAtributos()
        {
            var atributos = await _atributoService.GetAtributosAsync();
            return View(atributos);
        }


        [HttpGet]
        [Route("admin/ordenes")]
        public async Task<IActionResult> GetOrdenes()
        {
            var ordenes = await _ordenService.GetOrdenesUsuariosAsync();
            List<OrdenVM> result = ordenes.Select(o => new OrdenVM { Orden = o, Usuario = o.Usuario  }).ToList();
            return View(result);
        }

        [HttpGet]
        [Route("admin/reviews")]
        public async Task<IActionResult> GetReviews()
        {
            var reviews = await _reviewService.GetReviewAsync();
            return View(reviews);
        }

        [HttpGet]
        [Route("admin/carousel")]
        public async Task<IActionResult> Carousel()
        {
            var carouselImages = await _imagesService.GetCarouselImagesAsync();
            return View(carouselImages);
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
                await _reviewService.DeleteReviewAsync(reviewId);
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
                await _preguntaService.UpdatePreguntaRespuestaAsync(respuesta);
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
                await _imagesService.AddCarouselImageAsync(file.Name, fileStream);
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
                await _imagesService.UpdateCarouselImageAsync(imageId, file.Name, stream);
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

