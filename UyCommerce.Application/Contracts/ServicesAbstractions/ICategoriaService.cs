using UyCommerce.Application.DTOs;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
public interface ICategoriaService
    {
        Task<List<Categoria>> GetCategorias();
        Task<List<Categoria>> GetCategoriasInicio();
        Task<Categoria?> GetCategoriaById(int id);
        Task AddCategoria(CategoriaDTO categoriaDto, Stream? imgStream);
        Task FeatureCategoria(int categoriaId, bool feature);
        Task<List<Atributo>?> GetCategoriaAtributos(int id);
        Task<List<Categoria>> GetCategoriasComplete();
        
    }
}
