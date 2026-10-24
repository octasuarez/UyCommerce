using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Application.DTOs;
using UyCommerce.Domain.Entities;
using UyCommerce.Domain.Exceptions;
using UYCommerce.ViewModels;


namespace UYCommerce.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly ICategoriaService _categoriaService;
        public CategoriaController(ICategoriaService categoriaService) => this._categoriaService = categoriaService;

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("admin/categorias")]
        public async Task<IActionResult> GetCategorias()
        {
            var categorias = await _categoriaService.GetCategoriasCompleteAsync();
            return View("../Admin/GetCategorias", categorias);
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetCategoriasApi()
        {
            var categorias = await _categoriaService.GetCategoriasAsync();
            return Ok(categorias.Select(x => new { x.Id, x.Nombre }).ToList());
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetCategoriaAtributos(string categoriaId)
        {
            try
            {
                if (int.TryParse(categoriaId, out int id))
                {
                    var atributos = await _categoriaService.GetCategoriaAtributosAsync(id);
                    return Ok(atributos?.Select(a => new { a.Id, a.Nombre }).ToList());
                }
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            return BadRequest($"{categoriaId} no es una categoria valida.");
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
                return View(categoria);
            }
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
            try
            {
                var categoria = _categoriaService.FeatureCategoria(categoriaId, featured);
            }
            catch (NotFoundException e)
            {
                return BadRequest(e.Message);
            }
            return Ok("Categoria actualizada correctamente.");
        }
    }
}

