using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UYCommerce.Data;
using UYCommerce.DTOs;
using UYCommerce.Models;
using UYCommerce.Services;

namespace UYCommerce.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ShopContext _context;
    private readonly IEmailService _emailService;

    public HomeController(ILogger<HomeController> logger, ShopContext context, IEmailService emailService)
    {
        _logger = logger;
        _context = context;
        _emailService = emailService;
    }

    public async Task<IActionResult> Index()
    {

        var productos = await _context.Productos
            .Include(p => p.Reviews)
            .Include(p=> p.Skus)!.ThenInclude(s => s.Imagenes)
            .Where(p => p.Featured == true)
            .ToListAsync();

        var skus = productos
            .OrderByDescending(p => p.GetPuntuacionPromedio())
            .Select(p => p.Skus!.First()).ToList();

        HomeVM homeVM = new()
        {
            Skus = skus,
            Categorias = await _context.Categorias.Where(c => c.MostrarEnInicio == true).ToListAsync(),
            Carousel = await _context.CarouselImages.ToListAsync()
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
    public IActionResult Contacto() {

        return View();
    }

    [HttpPost]
    public IActionResult Contacto([FromBody]ContactoDTO contacto) {

        if (ModelState.IsValid) {

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

}

