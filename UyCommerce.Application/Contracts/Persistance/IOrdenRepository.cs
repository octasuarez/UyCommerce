using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.Persistance
{
    public interface IOrdenRepository
    {
        Task<List<Orden>> GetOrdenesPorFecha(DateTime startDate, DateTime endDate);
    }
}
