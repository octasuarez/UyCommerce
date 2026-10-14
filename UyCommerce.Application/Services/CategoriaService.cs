using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Application.Contracts.Persistance.Services;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Application.DTOs;
using UyCommerce.Domain.Entities;
using UyCommerce.Domain.Exceptions;

namespace UyCommerce.Application.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IAtributoRepository _atributoRepository;
        private readonly IFileService _fileService;

        public CategoriaService(ICategoriaRepository categoriaRepository, IAtributoRepository atributoRepository, IFileService fileService)
        {
            _categoriaRepository = categoriaRepository;
            this._atributoRepository = atributoRepository;
            this._fileService = fileService;
        }

        public async Task AddCategoriaAsync(CategoriaDTO categoriaDto, Stream? imgStream)
        {
            Categoria newCategoria = new();

            if (!categoriaDto.EsCategoriaPadre && categoriaDto.CategoriaPadreId <= 0)
                throw new Exception("Elige una categoria Padre!");

            if (categoriaDto.EsCategoriaPadre && categoriaDto.CategoriaPadreId > 0)
                throw new Exception("Una categoria padre no puede tener otra categoria padre");

            var categoriaDb = await _categoriaRepository.GetCategoriaByNameAsync(categoriaDto.Nombre);

            if (categoriaDb is not null)
                throw new Exception($"{nameof(Categoria)} already exists");

            Categoria? categoriaPadre = null;

            if (!categoriaDto.EsCategoriaPadre && categoriaDto.CategoriaPadreId > 0)
            {
                categoriaPadre = await _categoriaRepository.GetByIdAsync(categoriaDto.CategoriaPadreId)
                    ?? throw new NotFoundException(nameof(Categoria), categoriaDto.CategoriaPadreId);
                newCategoria.CategoriaPadre = categoriaPadre;
            }

            List<Atributo> atributos = [];

            if (categoriaDto.Atributos is not null && categoriaDto.Atributos.Count > 0)
            {
                foreach (var atributoId in categoriaDto.Atributos)
                {
                    var atributo = await _atributoRepository.GetByIdAsync(atributoId);
                    if (atributo is not null)
                        atributos.Add(atributo);
                }
                newCategoria.Atributos = atributos;
            }

            if (imgStream is not null && !string.IsNullOrEmpty(categoriaDto.ImagenNombre))
            {
                using var newStream = imgStream;
                bool saved = await _fileService.SaveFile(categoriaDto.ImagenNombre, imgStream, "Imagenes/Categorias");
                if (!saved)
                    throw new Exception("No se pudo guardar la imagen.");
            }

            newCategoria.Nombre = categoriaDto.Nombre;
            newCategoria.EsCategoriaPadre = categoriaDto.EsCategoriaPadre;
            newCategoria.MostrarEnInicio = categoriaDto.MostrarEnInicio;
            newCategoria.ImagenNombre = categoriaDto.ImagenNombre;
            newCategoria.Atributos = atributos;
            newCategoria.CategoriaPadre = categoriaPadre;

            await _categoriaRepository.CreateAsync(newCategoria);
        }
        public async Task<Categoria?> GetCategoriaByIdAsync(int id)
        {
            return await _categoriaRepository.GetByIdNoTrackingAsync(id);
        }

        public async Task<List<Categoria>> GetCategoriasAsync()
        {
            return await _categoriaRepository.GetAllAsync();
        }
        public async Task FeatureCategoria(int categoriaId, bool feature)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(categoriaId) 
                ?? throw new NotFoundException(nameof(Categoria), categoriaId);

            categoria.MostrarEnInicio = feature;
            await _categoriaRepository.UpdateAsync(categoria);
        }

        public async Task<List<Atributo>?> GetCategoriaAtributosAsync(int id)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Categoria), id);
            return await _atributoRepository.GetAtributosByCategoriaAsync(categoria);
        }

        public async Task<List<Categoria>> GetCategoriasCompleteAsync()
        {
            return await _categoriaRepository.GetCategoriasCompleteAsync();
        }

        public async Task<List<Categoria>> GetCategoriasInicioAsync()
        {
            return await _categoriaRepository.GetCategoriasInicioAsync();
        }
    }
}
