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

        public ProductoService(IProductoRepository productoRepository
            , ICategoriaRepository categoriaRepository
            , IMarcaRepository marcaRepository
            , IImagesService imagesService
            , IFileService fileService)
        {
            _productoRepository = productoRepository;
            this._categoriaRepository = categoriaRepository;
            this._marcaRepository = marcaRepository;
            this._imagesService = imagesService;
            this._fileService = fileService;
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
        public async Task<List<Producto>> GetFeaturedProductsAsync()
        {
            return await _productoRepository.GetFeaturedProductsAsync();
        }

        public async Task<Producto?> GetProductoCompleteByIdAsync(int id)
        {
            return await _productoRepository.GetProductoCompleteByIdAsync(id);
        }

        public async Task<GetProductsByQuery> GetProductosByQuery(string query, ProductsOrderBy orderBy, int pag , int qty)
        {
            return await _productoRepository.GetProductosByQueryAsync(query, orderBy, pag, qty);
        }

        public async Task<List<Producto>> GetProductosCompleteAsync()
        {
            return await _productoRepository.GetProductosCompleteAsync();
        }
    }
}
