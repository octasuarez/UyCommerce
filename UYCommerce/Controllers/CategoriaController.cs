using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UYCommerce.Data;
using UYCommerce.DTOs;
using UYCommerce.Models;
using UYCommerce.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UYCommerce.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly ShopContext _context;
        private readonly IFileService _fileService;

        public CategoriaController(ShopContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("admin/categorias")]
        public async Task<IActionResult> GetCategorias()
        {
            var categorias = await _context.Categorias
                .Include(c => c.Atributos)
                .Include(c => c.Productos)
                .ToListAsync();

            return View("../Admin/GetCategorias", categorias);
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetCategoriasApi()
        {

            return Ok(await _context.Categorias.Select(x => new { x.Id, x.Nombre }).ToListAsync());
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetCategoriaAtributos(string categoriaId)
        {

            var id = int.Parse(categoriaId);
            var categoria = await _context.Categorias.Include(c => c.Atributos).FirstOrDefaultAsync(c => c.Id == id);
            var atributos = categoria?.Atributos?.Select(a => new { a.Id, a.Nombre }).ToList();
            return Ok(atributos);
        }

        [HttpGet]
        [Route("admin/categorias/create")]
        public IActionResult CrearCategoria()
        {
            return View();
        }

        [HttpPost]
        [Route("admin/categorias/create")]
        public async Task<IActionResult> CrearCategoria(CategoriaDTO categoria)
        {

            if (ModelState.IsValid)
            {
                var categoriaDb = await _context.Categorias.FirstOrDefaultAsync(c => c.Nombre!.ToLower() == categoria.Nombre!.ToLower());

                if (categoriaDb is not null)
                    return BadRequest("Categoria ya existe");

                if (!categoria.EsCategoriaPadre && categoria.CategoriaPadreId <= 0)
                    return BadRequest("Elige una categoria Padre!");

                if (categoria.EsCategoriaPadre && categoria.CategoriaPadreId > 0)
                    return BadRequest("Una categoria padre no puede tener otra categoria padre");

                Categoria? categoriaPadre = new();

                if (!categoria.EsCategoriaPadre && categoria.CategoriaPadreId > 0)
                    categoriaPadre = await _context.Categorias.FirstOrDefaultAsync(c => c.Id == categoria.CategoriaPadreId);


                List<Atributo> atributos = new();

                if (categoria.Atributos is not null && categoria.Atributos.Count > 0)
                {
                    foreach (var a in categoria.Atributos)
                    {
                        var atributo = await _context.Atributos.FirstOrDefaultAsync(x => x.Id == a);
                        if (atributo is not null)
                            atributos.Add(atributo);
                    }
                }

                Categoria nuevaCategoria = new()
                {
                    Nombre = categoria.Nombre,
                    EsCategoriaPadre = categoria.EsCategoriaPadre,
                    MostrarEnInicio = categoria.MostrarEnInicio,
                    ImagenNombre = categoria.Imagen?.FileName,
                    Atributos = atributos,
                    CategoriaPadre = categoriaPadre
                };

                await _context.Categorias.AddAsync(nuevaCategoria);
                await _context.SaveChangesAsync();

                await _fileService.SaveFile(new List<IFormFile>() { categoria.Imagen! }, "Imagenes/Categorias");

                return Redirect("/admin/categorias");
            }

            return View(categoria);
        }
    }
}

