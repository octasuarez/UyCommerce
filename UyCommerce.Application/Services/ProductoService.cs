using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Common.Enums;
using UyCommerce.Application.Common.Models;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Application.Contracts.Persistance.Services;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Application.DTOs;
using UyCommerce.Domain.Entities;
using UyCommerce.Domain.Exceptions;

namespace UyCommerce.Application.Services
{
    public class ProductoService : IProductoService
    {
        private readonly string _imgFilesPath;
        private readonly IProductoRepository _productoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IMarcaRepository _marcaRepository;
        private readonly IImagesService _imagesService;
        private readonly IFileService _fileService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ISkuRepository _skuRepository;

        public ProductoService(IProductoRepository productoRepository
            , ICategoriaRepository categoriaRepository
            , IMarcaRepository marcaRepository
            , IImagesService imagesService
            , IFileService fileService
            , IUsuarioRepository usuarioRepository
            , ISkuRepository skuRepository)
        {
            _productoRepository = productoRepository;
            this._categoriaRepository = categoriaRepository;
            this._marcaRepository = marcaRepository;
            this._imagesService = imagesService;
            this._fileService = fileService;
            this._usuarioRepository = usuarioRepository;
            this._skuRepository = skuRepository;
            this._imgFilesPath = "Imagenes/Productos";
        }

        public async Task AddProductoAsync(AddProductoDto addProductoDto)
        {
            Producto newProducto = new();
            var productoDb = await _productoRepository.GetProductoByNombreAsync(addProductoDto.Nombre);
            if (productoDb != null)
                throw new ArgumentOutOfRangeException($"{nameof(Producto)} con nombre {addProductoDto.Nombre} ya existe.");

            var categoria = await _categoriaRepository.GetCategoriaByIdCompleteAsync(addProductoDto.CategoriaId)
                ?? throw new NotFoundException(nameof(Categoria), addProductoDto.CategoriaId);

            var marca = await _marcaRepository.GetByIdAsync(addProductoDto.MarcaId)
                ?? throw new NotFoundException(nameof(Marca), addProductoDto.MarcaId);

            var atributos = categoria?.Atributos?.Where(a => addProductoDto.Atributos.Contains(a.Id)).ToList();
            var nombreClave = addProductoDto.Nombre.ToLower().Replace(" ", "-");
            var imgFileNames = addProductoDto.Imagenes.Select(i => i.ImageName).ToList();

            if (_fileService.FilesExists(imgFileNames, _imgFilesPath))
                throw new ArgumentOutOfRangeException($"Imagen ya existe.");

            newProducto.Nombre = addProductoDto.Nombre;
            newProducto.NombreClave = nombreClave;
            newProducto.Descripcion = addProductoDto.Descripcion;
            newProducto.Categoria = categoria;
            newProducto.Marca = marca;
            newProducto.Atributos = atributos;
            newProducto.Imagenes = [];

            foreach (var img in addProductoDto.Imagenes)
            {
                if (img.Stream != null && img.Stream.Length > 0)
                {
                    await _imagesService.SaveImageFile(img.ImageName, img.Stream, _imgFilesPath);
                    var newProductoImagen = new ProductoImagen { ImagenNombre = img.ImageName };
                    newProducto.Imagenes.Add(newProductoImagen);
                }
            }
            await _productoRepository.CreateAsync(newProducto);
        }

        public async Task AddSkuAProductoAsync(AddSkuAProductoDto addSkuAProductoDto)
        {
            var producto = await _productoRepository.GetProductoCompleteByIdAsync(addSkuAProductoDto.ProductoId)
                ?? throw new NotFoundException(nameof(Producto), addSkuAProductoDto.ProductoId);

            if (producto.Atributos!.Any() && addSkuAProductoDto.AtributoValores is null
                || producto.Skus!.Any(s => s.AtributosValores!.All(a => addSkuAProductoDto.AtributoValores!.Any(av => av == a.Id)))
                || producto.Skus!.Any(s => s.Nombre.ToLower() == addSkuAProductoDto.Nombre.ToLower()))
            {
                throw new Exception("Atributos invalidos.");
            }

            var key = producto.NombreClave;
            List<AtributoValor> atributos = [];

            if (addSkuAProductoDto.AtributoValores.Count > 0 && producto.Atributos != null && producto.Atributos.Count > 0)
            {
                key += "/";
                atributos = producto.Atributos
                    .SelectMany(a => a.Valores!)
                    .Where(a => addSkuAProductoDto.AtributoValores
                    .Any(av => av == a.Id)).ToList();
                atributos.ForEach(a => key += "-" + a.Valor);
            }

            List<SkuImagen> skuImagenes = [];

            if (addSkuAProductoDto.Imagenes.Count > 0)
            {
                skuImagenes = [.. addSkuAProductoDto.Imagenes.Select(i => new SkuImagen { ImagenNombre = i.ImageName })];
            }
            else if (producto.Imagenes != null)
            {
                skuImagenes = [.. producto.Imagenes.Select(i => new SkuImagen { ImagenNombre = i.ImagenNombre })];
            }

            Sku newSku = new()
            {
                ProductoId = producto.Id,
                Key = key!.ToLower(),
                Nombre = addSkuAProductoDto.Nombre,
                Precio = addSkuAProductoDto.Precio,
                Imagenes = skuImagenes,
                AtributosValores = atributos,
                Stock = addSkuAProductoDto.Stock
            };

            if (addSkuAProductoDto.Imagenes.Count > 0)
            {
                foreach (var img in addSkuAProductoDto.Imagenes)
                {
                    if (img.Stream != null && img.Stream.Length > 0)
                    {
                        await _imagesService.SaveImageFile(img.ImageName, img.Stream, _imgFilesPath);
                        var newSkuImagen = new SkuImagen { ImagenNombre = img.ImageName };
                        newSku.Imagenes.Add(newSkuImagen);
                    }
                }
            }

            producto.Skus ??= [];
            producto.Skus.Add(newSku);
            await _productoRepository.UpdateAsync(producto);
        }

        public async Task DeleteSkuAsync(int skuId)
        {
            var sku = await _skuRepository.GetByIdAsync(skuId)
                ?? throw new NotFoundException(nameof(Sku), skuId);
            await _skuRepository.DeleteAsync(sku);
        }

        public async Task FeatureProductAsync(int id, bool feature)
        {
            var producto = await _productoRepository.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(Producto), id);
            producto.Featured = feature;
            await _productoRepository.UpdateAsync(producto);
        }

        public async Task<List<Producto>> GetFeaturedProductsAsync()
        {
            return await _productoRepository.GetFeaturedProductsAsync();
        }

        public async Task<Producto?> GetProductoCompleteByIdNoTrackingAsync(int id)
        {
            return await _productoRepository.GetProductoCompleteByIdNoTrackingAsync(id);
        }

        public async Task<GetProductsByQuery> GetProductosByQueryAsync(string query, ProductsOrderBy orderBy, int pag, int qty, int? userId)
        {
            var result = await _productoRepository.GetProductosByQueryAsync(query, orderBy, pag, qty);
            result.Categorias = await _categoriaRepository.GetCategoriasPadreAsync();
            if (userId != null)
            {
                var user = await _usuarioRepository.GetUsuarioAndFavoritos((int)userId);
                result.Favoritos = user != null && user.Favoritos != null ? (List<Sku>)user.Favoritos : [];
            }
            return result;
        }

        public async Task<List<GetProductosByQueryNameDto>> GetProductosByQueryNameAsync(string query)
        {
            var productos = await _productoRepository.GetProductosByQueryNameAsync(query);
            var result = productos.Select(p => new GetProductosByQueryNameDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                NombreClave = p.NombreClave,
                ImagenNombre = p.Imagenes!.First().ImagenNombre
            }).ToList();

            return result ??= [];
        }

        public async Task<List<Producto>> GetProductosCompleteAsync()
        {
            return await _productoRepository.GetProductosCompleteAsync();
        }

        public async Task<GetSkuByKeyDto> GetSkuByKeyAsync(string key, int? usuarioId)
        {
            GetSkuByKeyDto result = new();
            var sku = await _skuRepository.GetSkuByKeyAsync(key)
                ?? throw new NotFoundException(nameof(Sku), key);



            var skus = await _skuRepository.GetSkusByProductoIdAsync(sku.ProductoId);

            var reviews = sku.Producto.Reviews.Select(r => new Review()
            {
                Puntuacion = r.Puntuacion,
                Texto = r.Texto,
                Usuario = new Usuario() { Nombre = r.Usuario.Nombre }
            });
            var preguntas = sku.Producto.Preguntas.Select(p => new Pregunta()
            {
                Contenido = p.Contenido,
                Fecha = p.Fecha,
                Usuario = new Usuario() { Nombre = p.Usuario.Nombre },
                Respuesta = new Respuesta() { Contenido = p.Respuesta.Contenido, Fecha = p.Respuesta.Fecha }
            });
            sku.Producto.Reviews = [.. reviews];
            sku.Producto.Preguntas = [.. preguntas];

            SkuDto skuDto = new SkuDto()
            {
                AtributosValores = sku.AtributosValores,
                Id = sku.Id,
                Imagenes = sku.Imagenes,
                Key = sku.Key,
                Nombre = sku.Nombre,
                Precio = sku.Precio,
                PrecioAnterior = sku.PrecioAnterior,
                Producto = sku.Producto,
                ProductoId = sku.ProductoId,
                Stock = sku.Stock,
            };

            result.Sku = skuDto;
            result.Reviews = sku.Producto!.Reviews;
            result.Categoria = sku.Producto.Categoria;
            result.ProductosRelacionados = [.. sku.Producto.Categoria!.Productos!.SelectMany(p => p.Skus!)];
            result.PuntuacionPromedio = reviews != null && reviews.Count() > 0 ? reviews.Average(r => r.Puntuacion) : 0;

            if (usuarioId != null)
            {
                var usuario = await _usuarioRepository.GetUsuarioAndFavoritos((int)usuarioId)
                ?? throw new NotFoundException(nameof(Usuario), usuarioId);
                result.Favoritos = usuario.Favoritos;
            }

            if (sku.AtributosValores != null && sku.AtributosValores.Count > 0)
            {
                List<AtributoValor> opciones = skus
                                               .SelectMany(s => s.AtributosValores)
                                               .Select(a => new AtributoValor()
                                               {
                                                   Id = a.Id,
                                                   Valor = a.Valor,
                                                   Atributo = new Atributo()
                                                   {
                                                       Id = a.Atributo.Id,
                                                       Nombre = a.Atributo.Nombre,
                                                   }
                                               }).ToList();
                opciones = [.. opciones.Distinct()];
                var opcionesPorAtributo = opciones.GroupBy(a => a.Atributo.Id)
                                                  .Select(g => new AtributoDto
                                                  {
                                                      Nombre = g.First(a => a.Atributo.Id == g.Key).Atributo.Nombre,
                                                      Valores = g.GroupBy(g => g.Id).Select(v => v.First().Valor).ToList()
                                                  }).ToList();
                result.Opciones = opcionesPorAtributo;
            }

            return result;
        }

        public async Task<string> FindSkuKeyMatchAsync(string productoClave, string filters)
        {
            string skuKey = $"{productoClave}/{filters}";
            var skuKeyExists = await _skuRepository.CheckSkuKeyExists(skuKey);
            if (!skuKeyExists)
            {
                string[] skuParts = [productoClave, filters.Split("-").Last()];
                skuKey = await _skuRepository.FindSkyKeyByParts(skuParts) ?? string.Empty;
            }
            return skuKey;
        }
    }
}
