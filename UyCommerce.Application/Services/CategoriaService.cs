using MapsterMapper;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Application.Contracts.Persistance.Services;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Application.DTOs.Categorias;
using UyCommerce.Domain.Entities;
using UyCommerce.Domain.Exceptions;

namespace UyCommerce.Application.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IAtributoRepository _atributoRepository;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public CategoriaService(
            ICategoriaRepository categoriaRepository
            , IAtributoRepository atributoRepository
            , IFileService fileService
            , IMapper mapper)
        {
            _categoriaRepository = categoriaRepository;
            this._atributoRepository = atributoRepository;
            this._fileService = fileService;
            this._mapper = mapper;
        }

        public async Task AddCategoria(CreateCategoriaRequest categoriaDto, Stream? imgStream)
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
                await _fileService.SaveFile(categoriaDto.ImagenNombre, imgStream, "Imagenes/Categorias");
            }

            newCategoria.Nombre = categoriaDto.Nombre;
            newCategoria.EsCategoriaPadre = categoriaDto.EsCategoriaPadre;
            newCategoria.MostrarEnInicio = categoriaDto.MostrarEnInicio;
            newCategoria.ImagenNombre = categoriaDto.ImagenNombre;
            newCategoria.Atributos = atributos;
            newCategoria.CategoriaPadre = categoriaPadre;

            await _categoriaRepository.CreateAsync(newCategoria);
        }
        public async Task<GetCategoriaByIdResponse> GetCategoriaById(int id)
        {
            var categoria = await _categoriaRepository.GetByIdNoTrackingAsync(id)
                ?? throw new NotFoundException(nameof(Categoria), id);
            return _mapper.Map<GetCategoriaByIdResponse>(categoria);
        }

        public async Task<GetCategoriasResponse> GetCategorias()
        {
            var categorias = await _categoriaRepository.GetAllAsync();
            return new GetCategoriasResponse { Categorias = _mapper.Map<List<GetCategoriasDto>>(categorias) };
        }
        public async Task FeatureCategoria(int categoriaId, bool feature)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(categoriaId)
                ?? throw new NotFoundException(nameof(Categoria), categoriaId);

            categoria.MostrarEnInicio = feature;
            await _categoriaRepository.UpdateAsync(categoria);
        }

        public async Task<GetCategoriaAtributosResponse> GetCategoriaAtributos(int id)
        {
            GetCategoriaAtributosResponse response = new();
            var categoria = await _categoriaRepository.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Categoria), id);
            var atributos = await _atributoRepository.GetAtributosByCategoriaAsync(categoria);
            if (atributos != null && atributos.Count > 0)
            {
                response.Atributos = _mapper.Map<List<GetCategoriaAtributosAtributoDto>>(atributos);
            }
            return response;
        }

        public async Task<GetCategoriasCompleteResponse> GetCategoriasComplete()
        {
            var categorias = await _categoriaRepository.GetCategoriasCompleteAsync();
            return new GetCategoriasCompleteResponse { Categorias = _mapper.Map<List<GetCategoriasCompleteDto>>(categorias) };
        }
    }
}
