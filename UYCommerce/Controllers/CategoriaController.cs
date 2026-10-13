using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using UyCommerce.Application.Contracts.Persistance.Services;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Application.DTOs;
using UyCommerce.Domain.Entities;
using UyCommerce.Infrastructure.Persistance;
using UYCommerce.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UYCommerce.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly ShopContext _context;
        private readonly IFileService _fileService;
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ShopContext context, IFileService fileService, ICategoriaService categoriaService)
        {
            _context = context;
            _fileService = fileService;
            this._categoriaService = categoriaService;
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
        public async Task<IActionResult> CrearCategoria(CreateCategoriaModel categoria)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Categoria invalida.");
            }
            try
            {
                CategoriaDTO newCategoria = new CategoriaDTO
                {
                    Nombre = categoria.Nombre,
                    Atributos = categoria.Atributos,
                    CategoriaPadreId = categoria.CategoriaPadreId,
                    EsCategoriaPadre = categoria.EsCategoriaPadre,
                    MostrarEnInicio = categoria.MostrarEnInicio,
                };

                Stream? imgStream = null;
                
                if (categoria.Imagen is not null && categoria.Imagen.Length > 0)
                {
                    newCategoria.ImagenNombre = categoria.Imagen.Name;
                    imgStream = categoria.Imagen.OpenReadStream();
                }

                await _categoriaService.AddCategoriaAsync(newCategoria, imgStream);
                return Redirect("/admin/categorias");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return View(categoria);
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<Categoria?> GetCategoriaById(int categoriaId)
        {
            var categoria = await _categoriaService.GetCategoriaByIdAsync(categoriaId);
            return categoria;
        }


        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Feature(int categoriaId, bool featured)
        {
            var categoria = await GetCategoriaById(categoriaId);

            if (categoria is null)
                return BadRequest(new { msg = "Categoria not found" });

            categoria.MostrarEnInicio = featured;

            _context.Update(categoria);
            await _context.SaveChangesAsync();

            return Ok(new { msg = featured == true ? "Featured" : "Not featured" });
        }
    }
}

