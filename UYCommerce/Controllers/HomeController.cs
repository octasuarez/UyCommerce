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

        var productos = await _context.Productos.Include(p => p.Reviews)
            .Include(p=> p.Skus)!.ThenInclude(s => s.Imagenes)
            .ToListAsync();

        var skus = productos.OrderByDescending(p => p.GetPuntuacionPromedio())
            .Take(4).Select(p => p.Skus!.First()).ToList();

        HomeVM homeVM = new()
        {
            Skus = skus,
            Categorias = await _context.Categorias.Where(c => c.MostrarEnInicio == true).ToListAsync(),
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

}

