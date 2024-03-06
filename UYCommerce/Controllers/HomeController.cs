using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UYCommerce.Data;
using UYCommerce.Models;

namespace UYCommerce.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ShopContext _context;

    public HomeController(ILogger<HomeController> logger, ShopContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {

        HomeVM homeVM = new()
        {
            Productos = await _context.Productos
            .Include(p => p.Imagenes)
            .Include(p => p.Skus)!.ThenInclude(s => s.Imagenes)
            .Include(p => p.Skus)!.ThenInclude(s => s.AtributosValores)
            .ToListAsync(),

            Categorias = await _context.Categorias.Where(c => c.MostrarEnInicio == true).ToListAsync(),

            Favoritos = new List<Producto>()
        };

        var usuario = new Usuario();

        if (User.Identity is not null && User.Identity.IsAuthenticated)
        {
            usuario = _context.Usuarios.FirstOrDefault(u => u.Id.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        if(usuario is not null && usuario.Favoritos != null) {
            homeVM.Favoritos = usuario.Favoritos;
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

}

