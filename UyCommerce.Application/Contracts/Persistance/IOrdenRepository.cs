using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.Persistance
{
    public interface IOrdenRepository
    {
        Task<List<Orden>> GetOrdenesByDateAsync(DateTime startDate, DateTime endDate);
        Task<List<Orden>> GetOrdenesAsync();
    }
}
