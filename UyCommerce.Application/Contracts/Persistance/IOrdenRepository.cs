using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.Persistance
{
    public interface IOrdenRepository : IGenericRepository<Orden>
    {
        Task<List<Orden>> GetOrdenesByDateAsync(DateTime startDate, DateTime endDate);
        Task<List<Orden>> GetOrdenesAsync();
        Task<List<Orden>> GetOrdenesUsuariosAsync();
        Task<List<Orden>> GetOrdenesByProductoIdAsync(int productoId);
        Task<List<Orden>> GetOrdenesByUsuarioId(int id);
        Task<List<Producto>> GetProductosFromOrdenesByUsuarioId(int id);
    }
}
