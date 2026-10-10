using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UyCommerce.Application.DTOs;
using UyCommerce.Domain.Entities;
using UyCommerce.Infrastructure.Persistance;
using UyCommerce.Infrastructure.Services.Email;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UYCommerce.ViewModels;

namespace UYCommerce.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ShopContext _context;
    private readonly IEmailService _emailService;
    private readonly IProductoService _productoService;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly ICarouselImageRepository _carouselImageRepository;

    public HomeController(ILogger<HomeController> logger,
        ShopContext context,
        IProductoService productoService,
        ICategoriaRepository categoriaRepository,
        ICarouselImageRepository carouselImageRepository,
        IEmailService emailService)
    {
        _logger = logger;
        _context = context;
        _emailService = emailService;
        _productoService = productoService;
        _categoriaRepository = categoriaRepository;
        _carouselImageRepository = carouselImageRepository ;
    }

    public async Task<IActionResult> Index()
    {
        var productos = await _productoService.GetFeaturedProductsAsync();

        var skus = productos
            .OrderByDescending(p => p.GetPuntuacionPromedio())
            .Select(p => p.Skus!.First()).ToList();

        HomeVM homeVM = new()
        {
            Skus = skus,
            Categorias = await _categoriaRepository.GetCategoriasInicio(),
            Carousel = await _carouselImageRepository.GetAllAsync()
        };

        var usuario = _context.Usuarios.Include(u => u.Favoritos).FirstOrDefault(u => u.Id.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier));
        homeVM.Favoritos = usuario?.Favoritos;

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

        if (ModelState.IsValid)
        {

            Message message = new()
            {
                Content = contacto.Mensaje,
                Subject = contacto.Asunto,
                To = new MimeKit.MailboxAddress("octasuarezp@gmail.com", "octasuarezp@gmail.com")
            };

            _emailService.SendEmail(message);

            return Ok("Se envió correctamente");
        }

        var errors = string.Join("\n", ModelState.Values
        .SelectMany(v => v.Errors)
        .Select(e => e.ErrorMessage));

        return BadRequest(errors);
    }

    [HttpGet]
    [Route("About")]
    public IActionResult About() {
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

