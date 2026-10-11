using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IReviewService
    {
        Task<List<Review>> GetReviewAsync();
        Task DeleteReviewAsync(int id);
    }

}
