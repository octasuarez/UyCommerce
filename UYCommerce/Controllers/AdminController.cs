using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using UyCommerce.Application.Contracts.Persistance.Services;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Application.DTOs;
using UyCommerce.Domain.Entities;
using UyCommerce.Domain.Exceptions;
using UyCommerce.Infrastructure.Persistance;
using UYCommerce.ViewModels;


namespace UYCommerce.Controllers
{
    [Authorize(Policy = "Admin")]
    public class AdminController : Controller
    {
        private readonly ShopContext _context;
        private readonly IFileService _fileService;
        private readonly IOrdenService _ordenService;
        private readonly IPreguntaService _preguntaService;
        private readonly IUsuarioService _usuarioService;
        private readonly IProductoService _productoService;
        private readonly IReviewService _reviewService;
        private readonly IAtributoService _atributoService;
        private readonly IImagesService _imagesService;

        public AdminController(ShopContext context,
            IFileService fileService
            , IOrdenService ordenService
            , IPreguntaService preguntaService
            , IUsuarioService usuarioService
            , IProductoService productoService
            , IReviewService reviewService
            , IAtributoService atributoService
            , IImagesService imagesService)
        {
            _context = context;
            _fileService = fileService;
            _ordenService = ordenService;
            _preguntaService = preguntaService;
            _usuarioService = usuarioService;
            _productoService = productoService;
            this._reviewService = reviewService;
            this._atributoService = atributoService;
            this._imagesService = imagesService;

        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard(DateTime? fchDesde, DateTime? fchHasta)
        {
            var ordenes = await _ordenService.GetOrdenesByDateAsync(fchDesde, fchHasta);

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
            var filter = orden.Equals("sin responder") ? false : true;
            var preguntas = await _preguntaService.GetPreguntas(filter);
            ViewBag.selected = orden;

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

            var result = await _productoService.GetProductosCompleteAsync();
            return View(result);
        }

        [HttpGet]
        [Route("admin/atributos")]
        public async Task<IActionResult> GetAtributos()
        {
            var result = await _atributoService.GetAtributosAsync();
            return View(result);
        }


        [HttpGet]
        [Route("admin/ordenes")]
        public async Task<IActionResult> GetOrdenes()
        {
            var ordenes = await _ordenService.GetOrdenesAsync();
            List<OrdenVM> result = ordenes.Join(_context.Usuarios, o => o.UsuarioId, u => u.Id, (o, u) => new OrdenVM { Orden = o, Usuario = u }).ToList();
            return View(result);
        }

        [HttpGet]
        [Route("admin/reviews")]
        public async Task<IActionResult> GetReviews()
        {
            var result = await _reviewService.GetReviewAsync();
            return View(result);
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

            return Ok(new { respuestaNueva.Contenido, respuestaNueva.PreguntaId, respuestaNueva.Fecha });

        }

        [HttpGet]
        [Route("admin/carousel")]
        public async Task<IActionResult> Carousel()
        {
            var carouselImages = await _imagesService.GetCarouselImagesAsync();
            return View(carouselImages);
        }

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
        public async Task<IActionResult> CreateCarouselImage(IFormFile file)
        {
            if (file is null || file.Length == 0)
            {
                return BadRequest("Imagen invalida.");
            }

            using var fileStream = file.OpenReadStream();
            await _imagesService.AddCarouselImageAsync(file.Name, fileStream);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeCarouselImage(IFormFile file, int imageId)
        {

            var image = await _context.CarouselImages.FirstOrDefaultAsync(i => i.Id == imageId);

            if (image is null)
                return BadRequest();

            image.Name = file.FileName;

            _context.Update(image);
            await _context.SaveChangesAsync();
            using var imgStream = file.OpenReadStream();
            await _fileService.SaveFile(file.Name, imgStream , "Imagenes/Carousel");
            _fileService.DeleteFile(new List<string>() { image.Name }, "Imagenes/Carousel");

            return Ok();
        }
    }
}

