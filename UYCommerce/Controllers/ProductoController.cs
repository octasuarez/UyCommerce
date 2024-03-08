using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using UYCommerce.Data;
using UYCommerce.Models;

namespace UYCommerce.Controllers
{
    public class ProductoController : Controller
    {
        private readonly ShopContext _context;

        public ProductoController(ShopContext context)
        {

            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        [Route("/search")]
        public async Task<IActionResult> BuscarProductos(string? query, string? categoria, string[]? filtros)
        {
            BusquedaProductoVM result = new();

            IQueryable<Producto> productos = _context.Productos
             .Include(p => p.Categoria)
             .ThenInclude(c => c!.CategoriaPadre)
             .Include(p => p.Imagenes)
             .Include(p => p.Skus)!.ThenInclude(s => s.Imagenes)
             .Include(p => p.Skus)!.ThenInclude(s => s.AtributosValores)!.ThenInclude(atv => atv.Atributo);

            if (!string.IsNullOrEmpty(query))
            {
                productos = productos.Where(p => p.Nombre!.ToLower().StartsWith(query.ToLower()));
            }
            else if (!string.IsNullOrEmpty(categoria)) //si no hay search query pero si categoria
            {

                result.Categoria = _context.Categorias
                    .Include(c => c.CategoriaPadre)
                    .Include(c => c.Atributos)!.ThenInclude(a => a.Valores)
                    .Include(c => c.SubCategorias).FirstOrDefault(c => c.Nombre.ToLower() == categoria.ToLower());

                if (result.Categoria != null)
                {
                    productos = productos.Where(p => p.Categoria!.Nombre!.ToLower() == categoria.ToLower()
                    || p.Categoria.CategoriaPadre!.Nombre!.Equals(categoria));
                }

                if (filtros is not null && filtros.Any())
                {
                    var atributosValores = new List<KeyValuePair<string, string>>();

                    for (int i = 0; i < filtros.Count(); i++)
                    {
                        if (filtros[i].Contains('-'))
                        {
                            var atributoValor = filtros[i].Split("-");
                            atributosValores.Add(new KeyValuePair<string, string>(atributoValor[0].ToLower(), atributoValor[1].ToLower()));
                        }
                    }

                    result.Skus = productos.SelectMany(p => p.Skus).ToList();
                    result.Skus =
                        result.Skus.Where(s => atributosValores.All(atr => s.AtributosValores.Any(av => av.Atributo.Nombre.ToLower() == atr.Key && av.Valor.ToLower() == atr.Value))).ToList();

                }
            }
            if (!filtros.Any())
            {
                result.Skus = productos.SelectMany(p => p.Skus).ToList();
            }

            result.Categorias = await _context.Categorias.Where(c => c.CategoriaPadre == null)
                .Select(x => new Categoria { Id = x.Id, Nombre = x.Nombre }).ToListAsync();

            return View("BusquedaProductos", result);
        }


        /// <summary>
        /// Encuentra el producto segun su NombreClave (key) y segun las opciones seleccionadas del cliente.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="options"></param>
        /// <returns>verProductoViewModel</returns>
        [HttpGet]
        [Route("/{key}")]
        public async Task<IActionResult> BuscarProductoPorKey(string key)
        {

            var sku = await _context.Skus
                .Include(s => s.AtributosValores)!.ThenInclude(av => av.Atributo)
                .Include(s => s.Producto).ThenInclude(p => p.Preguntas)
                .Include(s => s.Producto).ThenInclude(p => p.Reviews)
                .Include(s => s.Producto).ThenInclude(p => p.Marca)
                .Include(s => s.Imagenes)
                .FirstOrDefaultAsync(s => s.Nombre.Contains(key));

            if (sku is not null)
            {
                var skus = _context.Skus
                    .Where(s => s.ProductoId == sku.ProductoId)
                    .Include(s => s.AtributosValores)!.ThenInclude(a => a.Atributo).ToList();

                List<AtributoValor> opciones = skus.SelectMany(s => s.AtributosValores!.Where(a => a.Atributo!.Id == sku.AtributosValores!.First().Atributo!.Id)).ToList();

                opciones.AddRange(skus.Where(s => s.AtributosValores!.Contains(sku.AtributosValores!.First())).SelectMany(s => s.AtributosValores!).ToList());

                opciones = opciones.Distinct().ToList();
                var opcionesPorAtributo = opciones.GroupBy(a => a.Atributo);

                VerProductoVM result = new()
                {
                    Sku = sku,
                    Opciones = opcionesPorAtributo,

                };

                return View("VerProducto", result);
            }

            return Redirect("/NotFound");
        }

        /// <summary>
        /// Formatea las opciones de un producto
        /// </summary>
        /// <param name="opciones"></param>
        /// <returns>Diccionario de valores string</returns>
        private Dictionary<string, string>? FormatearOpcionesProductoQuery(string? opciones)
        {

            if (!string.IsNullOrEmpty(opciones))
            {
                string[] atributosFiltro = opciones.Contains(',') ? opciones.Split(",") : new string[] { opciones };

                var atributosValores = new Dictionary<string, string>();

                foreach (var a in atributosFiltro)
                {
                    if (a.Contains(':'))
                    {
                        var atributoValor = a.Split(":");
                        atributosValores.Add(atributoValor[0], atributoValor[1]);
                    }
                }

                return atributosValores;
            }

            return null;
        }

        /// <summary>
        /// Busca un producto por su nombre clave
        /// </summary>
        /// <param name="nombreClave"></param>
        /// <returns>Producto o Null</returns>
        private async Task<Producto?> BuscarProductoPorClave(string nombreClave)
        {

            Producto? producto = await _context.Productos
                .Include(p => p.Imagenes)
                .Include(p => p.Preguntas)!.ThenInclude(pr => pr.Respuesta)
                .Include(p => p.Preguntas)!.ThenInclude(pr => pr.Usuario)
                .Include(p => p.Categoria).ThenInclude(c => c!.Productos)
                .Include(p => p.Marca)
                .Include(p => p.Skus)!.ThenInclude(s => s.Imagenes)
                .Include(p => p.Skus)!.ThenInclude(s => s.AtributosValores)!.ThenInclude(a => a.Atributo)
                .FirstOrDefaultAsync(p => p.NombreClave == nombreClave);

            return producto;
        }
        /// <summary>
        /// Retrona los productos que esten dentro de la categoria especificada
        /// </summary>
        /// <param name="nombreCategoria"></param>
        /// <returns></returns>

        [HttpGet]
        [Route("/c/{nombreCategoria}")]
        public async Task<IActionResult> BuscarProductoPorCategoria(string? nombreCategoria)
        {
            var categoria = await _context.Categorias
                .Include(c => c.CategoriaPadre)
                .Include(c => c.SubCategorias)
                .FirstOrDefaultAsync(c => c.Nombre == nombreCategoria);

            if (categoria is null)
            {
                return View("../Home/Index");
            }

            var productos = await _context.Productos
                .Include(p => p.Imagenes)
                .Include(p => p.Skus)
                .Where(p => p.Categoria == categoria || p.Categoria!.CategoriaPadre!.Nombre == nombreCategoria)
                .ToListAsync();

            var categorias = await _context.Categorias.Where(c => c.CategoriaPadre == null)
                .Select(x => new Categoria { Id = x.Id, Nombre = x.Nombre }).ToListAsync();

            BusquedaProductoVM result = new() { Categorias = categorias, Productos = productos, Categoria = categoria };

            return View("BusquedaProductos", result);
        }

        [HttpPost]
        public async Task<JsonResult> Autocomplete(string query)
        {

            var productos = await _context.Productos
                .Include(p => p.Imagenes)
                .Where(p => p.Nombre!.ToLower().StartsWith(query.ToLower())
                || p.Nombre.ToLower().Contains(query.ToLower()))
                .Select(p => new
                {
                    id = p.Id,
                    nombre = p.Nombre,
                    nombreclave = p.NombreClave,
                    img = p.Imagenes!.First().ImagenNombre
                }).ToListAsync();

            return Json(productos);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarAFavoritos(string productoId)
        {

            if (User.Identity is null || User.Identity!.IsAuthenticated == false)
            {
                return new BadRequestObjectResult("El usuario no esta autenticado");
            }

            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id.ToString() == productoId);

            if (producto is null)
            {
                return new BadRequestObjectResult("El producto no fue encontrado");
            }

            var usuario = await _context.Usuarios.Include(u => u.Favoritos)
                .FirstOrDefaultAsync(u => u.Id.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier));

            string msj = "";
            if (usuario is null)
            {
                return new BadRequestObjectResult("El producto no fue encontrado");
            }
            else
            {
                if (usuario.Favoritos!.Contains(producto))
                {
                    usuario.Favoritos.Remove(producto);
                    msj = "eliminado";
                }
                else
                {
                    usuario.Favoritos.Add(producto);
                    msj = "agregado";

                }

            }

            _context.Update(usuario);
            await _context.SaveChangesAsync();

            return Json(new { msj });

        }

    }
}


