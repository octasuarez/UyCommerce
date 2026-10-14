using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.Persistance
{
    public interface ICategoriaRepository : IGenericRepository<Categoria>
    {
        public Task<List<Categoria>> GetCategoriasInicioAsync();
        public Task<Categoria?> GetCategoriaByNameAsync(string name);
        Task<List<Categoria>> GetCategoriasCompleteAsync();
    }
}
