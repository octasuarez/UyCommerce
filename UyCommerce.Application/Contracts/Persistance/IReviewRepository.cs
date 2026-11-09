using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.Persistance
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<List<Review>> GetReviewsAsync();
        Task<List<Review>> GetReviewsByUsuarioId(int id);
    }
}