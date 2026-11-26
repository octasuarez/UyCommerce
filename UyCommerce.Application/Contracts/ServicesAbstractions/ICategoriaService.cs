using UyCommerce.Application.DTOs.Categorias;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
public interface ICategoriaService
    {
        Task<GetCategoriasResponse> GetCategorias();
        Task<GetCategoriaByIdResponse> GetCategoriaById(int id);
        Task AddCategoria(CreateCategoriaRequest categoriaDto, Stream? imgStream);
        Task FeatureCategoria(int categoriaId, bool feature);
        Task<GetCategoriaAtributosResponse> GetCategoriaAtributos(int id);
        Task<GetCategoriasCompleteResponse> GetCategoriasComplete();
        
    }
}
