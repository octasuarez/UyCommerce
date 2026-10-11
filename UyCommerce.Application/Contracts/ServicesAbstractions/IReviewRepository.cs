using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<List<Review>> GetReviewsAsync();
    }
}