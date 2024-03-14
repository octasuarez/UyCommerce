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
        public async Task<IActionResult> BuscarProductosPorQuery(string query, int orden = 0)
        {
            BusquedaProductoVM result = new();

            IQueryable<Producto> productos = _context.Productos
             .Include(p => p.Categoria)
             .ThenInclude(c => c!.CategoriaPadre)
             .Include(p => p.Imagenes)
             .Include(p => p.Skus)!.ThenInclude(s => s.Imagenes)
             .Include(p => p.Skus)!.ThenInclude(s => s.Producto).ThenInclude(p => p.Reviews)
             .Include(p => p.Skus)!.ThenInclude(s => s.AtributosValores)!.ThenInclude(atv => atv.Atributo);

            productos = productos.Where(p => p.Nombre!.ToLower().StartsWith(query.ToLower()));

            result.Skus = await productos.SelectMany(p => p.Skus!).ToListAsync();

            result.Categorias = await _context.Categorias.Where(c => c.CategoriaPadre == null)
                .Select(x => new Categoria { Id = x.Id, Nombre = x.Nombre }).ToListAsync();

            if (User.Identity!.IsAuthenticated)
            {
                result.Favoritos = _context.Usuarios.Where(u => u.Id.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier)).SelectMany(u => u.Favoritos!).ToList();
            }

            if (orden > 0)
                result.Skus = OrdenarSkus(result.Skus, orden);

            return View("BusquedaProductos", result);
        }

        [HttpGet]
        [Route("/c/{categoria}")]
        public async Task<IActionResult> BuscarProductosPorCategoria(string categoria, string[]? filtros, int orden = 0)
        {
            BusquedaProductoVM result = new()
            {
                Categoria = _context.Categorias
                .Include(c => c.CategoriaPadre)
                .Include(c => c.Atributos)!.ThenInclude(a => a.Valores)
                .Include(c => c.SubCategorias).FirstOrDefault(c => c.Nombre.ToLower() == categoria.ToLower())
            };

            if (result.Categoria != null)
            {

                IQueryable<Producto> productos = _context.Productos
                 .Include(p => p.Categoria)
                 .ThenInclude(c => c!.CategoriaPadre)
                 .Include(p => p.Imagenes)
                 .Include(p => p.Skus)!.ThenInclude(s => s.Imagenes)
                 .Include(p => p.Skus)!.ThenInclude(s => s.Producto).ThenInclude(p => p.Reviews)
                 .Include(p => p.Skus)!.ThenInclude(s => s.AtributosValores)!.ThenInclude(atv => atv.Atributo);

                productos = productos.Where(p => p.Categoria!.Nombre!.ToLower() == categoria.ToLower()
                || p.Categoria.CategoriaPadre!.Nombre!.Equals(categoria));

                result.Skus = await productos.SelectMany(p => p.Skus!).ToListAsync();

                var atributosValores = filtros != null && filtros.Any() ? GetAtributosDeFiltros(filtros) : null;

                if (atributosValores != null)
                {
                    result.Skus = result.Skus.Where(s => atributosValores.All(atr => s.AtributosValores!.Any(av => av.Atributo!.Nombre!.ToLower() == atr.Key && av.Valor!.ToLower() == atr.Value))).ToList();
                    result.Atributos = atributosValores;
                }
                else { result.Skus = result.Skus.DistinctBy(s => s.ProductoId).ToList(); }

                result.Categorias = await _context.Categorias.Where(c => c.CategoriaPadre == null).Select(x => new Categoria { Id = x.Id, Nombre = x.Nombre }).ToListAsync();

                if (User.Identity!.IsAuthenticated)
                    result.Favoritos = _context.Usuarios.Where(u => u.Id.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier)).SelectMany(u => u.Favoritos!).ToList();

                if (orden > 0)
                    result.Skus = OrdenarSkus(result.Skus, orden);

                return View("BusquedaProductos", result);
            }
            return View("NotFound");
        }

        private static ICollection<Sku> OrdenarSkus(ICollection<Sku> skus, int orden)
        {

            switch (orden)
            {
                case 1:
                    skus = skus.OrderByDescending(s => s.Precio).ToList();
                    break;
                case 2:
                    skus = skus.OrderBy(s => s.Precio).ToList();
                    break;
                case 3:
                    skus = skus.OrderByDescending(s => s.Producto!.GetPuntuacionPromedio()).ToList();
                    break;
                default:
                    break;
            }

            return skus;
        }

        private static List<KeyValuePair<string, string>> GetAtributosDeFiltros(string[] filtros)
        {
            List<KeyValuePair<string, string>> atributosValores = new();

            for (int i = 0; i < filtros.Length; i++)
            {
                if (filtros[i].Contains('-'))
                {
                    var atributoValor = filtros[i].Split("-");
                    atributosValores.Add(new KeyValuePair<string, string>(atributoValor[0].ToLower(), atributoValor[1].ToLower()));
                }
            }
            return atributosValores;
        }

        /// <summary>
        /// Encuentra el producto segun su NombreClave (key) y segun las opciones seleccionadas del cliente.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="options"></param>
        /// <returns>verProductoViewModel</returns>
        [HttpGet]
        [Route("/{key}")]
        public async Task<IActionResult> GetProductoPorKey(string key)
        {

            var sku = await _context.Skus
                .Include(s => s.AtributosValores)!.ThenInclude(av => av.Atributo)
                .Include(s => s.Producto).ThenInclude(p => p!.Reviews)!.ThenInclude(r => r.Usuario)
                .Include(s => s.Producto).ThenInclude(p => p!.Preguntas)
                .Include(s => s.Producto).ThenInclude(p => p!.Reviews)
                .Include(s => s.Producto).ThenInclude(p => p!.Marca)
                .Include(s => s.Producto).ThenInclude(p => p!.Categoria).ThenInclude(c => c!.CategoriaPadre)
                .Include(s => s.Imagenes)
                .FirstOrDefaultAsync(s => s.Key.Contains(key));

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
                    Reviews = sku.Producto.Reviews,
                    Categoria = sku.Producto.Categoria
                };

                return View("VerProducto", result);
            }

            return Redirect("/NotFound");
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
        public async Task<IActionResult> AgregarAFavoritos(string skuId)
        {

            if (User.Identity is null || User.Identity!.IsAuthenticated == false)
            {
                return new BadRequestObjectResult("El usuario no esta autenticado");
            }

            var sku = await _context.Skus.FirstOrDefaultAsync(s => s.Id.ToString() == skuId);

            if (sku is null)
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
                if (usuario.Favoritos!.Contains(sku))
                {
                    usuario.Favoritos.Remove(sku);
                    msj = "eliminado";
                }
                else
                {
                    usuario.Favoritos.Add(sku);
                    msj = "agregado";

                }

            }

            _context.Update(usuario);
            await _context.SaveChangesAsync();

            return Json(new { msj });

        }

    }
}


