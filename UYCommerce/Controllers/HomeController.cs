using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using UyCommerce.Application.Contracts.Persistance.Services;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Application.DTOs;
using UyCommerce.Application.DTOs.Productos;

namespace UYCommerce.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IEmailService _emailService;
    private readonly IProductoService _productoService;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _config;

    public HomeController(
        IMemoryCache cache,
        ILogger<HomeController> logger,
        IProductoService productoService,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _logger = logger;
        _emailService = emailService;
        _productoService = productoService;
        _cache = cache;
        _config = configuration;
    }

    public async Task<IActionResult> Index()
    {
        GetFeaturedProductsResponse result = new();
        try
        {
            if (_cache.TryGetValue<GetFeaturedProductsResponse>("Productos", out var homeCache) && homeCache.Skus.Any())
            {
                result = homeCache;
            }
            else
            {
                int? userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int id) ? id : null;
                result = await _productoService.GetFeaturedProducts(userId);
                _cache.Set("Productos", result, TimeSpan.FromHours(1));
            }

        }
        catch (Exception)
        {
        }

        return View(result);
    }

    [HttpGet]
    [Route("/Contacto")]
    public IActionResult Contacto()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Contacto([FromBody] ContactoModel contacto)
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
            var email = _config["ContactEmail"];
            if (string.IsNullOrEmpty(email)) throw new Exception("Contact email not set");
            _emailService.SendEmail(email, contacto.Asunto!, contacto.Mensaje!);
        }
        catch (Exception e)
        {
            return StatusCode(500, "No se pudo enviar el mail");
        }
        return Ok("Se envió correctamente");
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

    [Route("NotFound")]
    public IActionResult NotFoundPage()
    {
        return View("NotFound");
    }

}

