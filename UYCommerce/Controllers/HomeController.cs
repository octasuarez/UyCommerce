using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using UyCommerce.Application.Contracts.Persistance.Services;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Application.DTOs;
using UyCommerce.Domain.Entities;
using UYCommerce.ViewModels;

namespace UYCommerce.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IEmailService _emailService;
    private readonly IProductoService _productoService;
    private readonly IUsuarioService _usuarioService;
    private readonly ICategoriaService _categoriaService;
    private readonly IImagesService _imagesService;

    public HomeController(ILogger<HomeController> logger,
        IProductoService productoService,
        IUsuarioService usuarioService,
        ICategoriaService categoriaService,
        IImagesService imagesService,
        IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
        _productoService = productoService;
        _usuarioService = usuarioService;
        _categoriaService = categoriaService;
        _imagesService = imagesService;
    }

    public async Task<IActionResult> Index()
    {
        HomeVM homeVM = new HomeVM();
        try
        {
            var productos = await _productoService.GetFeaturedProductsAsync();

            var skus = productos
                .OrderByDescending(p => p.GetPuntuacionPromedio())
                .Select(p => p.Skus!.First()).ToList();

            homeVM.Skus = skus;
            homeVM.Categorias = await _categoriaService.GetCategoriasInicioAsync();
            homeVM.Carousel = await _imagesService.GetCarouselImagesAsync();

            if (User.Identity is not null && User.Identity.IsAuthenticated
                && int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                var usuario = await _usuarioService.GetUsuarioFavoritosByIdAsync(userId);
                homeVM.Favoritos = usuario?.Favoritos;
            }
        }
        catch (Exception)
        {
        }

        return View(homeVM);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


    [Route("NotFound")]
    public IActionResult NotFoundPage()
    {
        return View("NotFound");
    }

    [HttpGet]
    [Route("/Contacto")]
    public IActionResult Contacto()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Contacto([FromBody] ContactoDTO contacto)
    {
        if (!ModelState.IsValid)
        {
            var errors = string.Join("\n", ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage));
            return BadRequest(errors);
        }
        try
        {
            _emailService.SendEmail("octasuarezp@gmail.com", contacto.Asunto!, contacto.Mensaje!);
        }
        catch (Exception e)
        {
            return StatusCode(500, "No se pudo enviar el mail");
        }
        return Ok("Se envió correctamente");
    }

    [HttpGet]
    [Route("About")]
    public IActionResult About()
    {
        return View();
    }

    [HttpGet]
    [Route("Faq")]
    public IActionResult Faq()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );

        return LocalRedirect(returnUrl);
    }

}

