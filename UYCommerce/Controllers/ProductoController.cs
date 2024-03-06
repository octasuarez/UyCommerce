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
                    var atributosValores = new List<KeyValuePair<string,string>>();

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
            if(!filtros.Any()) {
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
        public async Task<IActionResult> BuscarProductoPorKey(string key, string? options)
        {
            Producto? producto = await BuscarProductoPorClave(key);

            if (producto != null && producto.Skus != null && producto.Categoria != null && producto.Imagenes != null)
            {
                VerProductoVM verProductoVM = new()
                {
                    Producto = producto,
                    ProductosRelacionados = producto.Categoria.Productos,
                    Imagenes = producto.Imagenes.Select(i => i.ImagenNombre).ToList()!,
                    Preguntas = producto.Preguntas
                };

                if (producto.Skus.Count > 1)
                {
                    verProductoVM.PrecioMin = producto.Skus.Min(s => s.Precio);
                    verProductoVM.PrecioMax = producto.Skus.Max(s => s.Precio);
                }

                Dictionary<string, string>? opciones = new();

                if (!string.IsNullOrEmpty(options))
                {
                    opciones = FormatearOpcionesProductoQuery(options.ToLower());

                    if (opciones is not null)
                    {
                        verProductoVM.OpcionesElegidas = opciones;
                        var skuElegido = producto.Skus.FirstOrDefault(s => s.AtributosValores!.All(a => opciones.Values.Contains(a.Valor!.ToLower())));
                        verProductoVM.Sku = skuElegido;
                        if (skuElegido != null)
                        {
                            if (skuElegido.Imagenes != null && skuElegido.Imagenes.Any())
                            {
                                verProductoVM.Imagenes = skuElegido.Imagenes.Select(i => i.ImagenNombre).ToList()!;
                            }
                            verProductoVM.Precio = skuElegido.Precio;
                        }
                    }
                }

                verProductoVM.Atributos = FiltrarAtributosDeProductoSkus(producto.Skus, opciones);
                verProductoVM.Opciones = CrearOpciones(verProductoVM.Atributos, opciones);

                return View("VerProducto", verProductoVM);
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
        /// Crea las opciones para el usuario poder especificar los atributos que quiere de un producto
        /// </summary>
        /// <returns>Lista de AtributoOpcion</returns>
        private List<AtributoOpcion> CrearOpciones(ICollection<Atributo> atributos, Dictionary<string, string>? opciones)
        {

            List<AtributoOpcion> AtributosOpciones = new();

            string linkReferencia = "";

            for (int i = 0; i < atributos.Count; i++)
            {
                Atributo? atributoElegido = null;

                var opcion = i > (opciones!.Count - 1) ? default(KeyValuePair<string, string>) : opciones!.ElementAt(i);

                if (!opcion.Equals(default(KeyValuePair<string, string>)))
                {
                    atributoElegido = atributos.FirstOrDefault(a => a.Nombre!.ToLower() == opcion.Key.ToLower());
                }

                if (atributoElegido == null)
                {
                    var atributosYaExisten = AtributosOpciones.Select(s => s.AtributoNombre).Distinct();
                    atributoElegido = atributos.FirstOrDefault(a => !atributosYaExisten.Contains(a.Nombre!));
                }

                AtributoOpcion AtrOpcionNuevo = new() { AtributoNombre = atributoElegido!.Nombre };

                if (atributoElegido is not null && atributoElegido.Valores is not null)
                {

                    foreach (var atributoValor in atributoElegido.Valores)
                    {
                        string link = "";

                        if (string.IsNullOrEmpty(linkReferencia))
                        {
                            link = $"?options={atributoValor.Atributo!.Nombre}:{atributoValor.Valor}";
                        }
                        else
                        {
                            link = linkReferencia + $",{atributoValor.Atributo!.Nombre}:{atributoValor.Valor}";
                        }

                        AtrOpcionNuevo.Valores.Add(atributoValor.Valor!.ToLower(), link);
                    }

                    AtributosOpciones.Add(AtrOpcionNuevo);

                    if (opcion.Key != null)
                    {
                        linkReferencia = AtrOpcionNuevo.Valores.FirstOrDefault(s => s.Key.ToLower() == opcion.Value.ToLower()).Value;
                    }

                }
            }
            return AtributosOpciones;
        }

        /// <summary>
        /// Retorna los atributos de los skus de un producto, agrupados por tipo y considernado las opciones elegidas.
        /// </summary>
        /// <param name="productoSkus"></param>
        /// <param name="opciones"></param>
        /// <returns></returns>
        private List<Atributo> FiltrarAtributosDeProductoSkus(ICollection<Sku> productoSkus, Dictionary<string, string>? opciones)
        {

            var skus = new List<Sku>();

            List<AtributoValor> atributosValores = new();

            if (opciones is not null)
            {
                for (int i = 1; i < opciones.Count + 1; i++)
                {
                    foreach (var s in productoSkus)
                    {
                        atributosValores.AddRange(s.AtributosValores!.Where(a => a.Atributo!.Nombre!.ToLower() == opciones.First().Key));

                        var skusAtributos = s.AtributosValores!.ToDictionary(a => a.Atributo!.Nombre!.ToLower(), a => a.Valor!.ToLower());

                        var skuTieneOpciones = opciones.Take(i).All(o => skusAtributos!.ContainsKey(o.Key) && skusAtributos[o.Key] == o.Value);

                        if (skuTieneOpciones)
                        {
                            skus.Add(s);
                        }
                    }
                }
            }

            atributosValores.AddRange(skus.Any() ? skus.SelectMany(s => s.AtributosValores!) : productoSkus.SelectMany(s => s.AtributosValores!));

            var atributosAgrupados = (from atributoValor in atributosValores
                                      group atributoValor by atributoValor.Atributo into a
                                      orderby a.Key.Nombre
                                      select new Atributo() { Nombre = a.Key.Nombre, Valores = a.Distinct().ToList() }).ToList();

            return atributosAgrupados;
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


