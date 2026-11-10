using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.DTOs.Reviews;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IReviewService
    {
        Task<List<Review>> GetReview();
        Task DeleteReview(int id);
        Task CreateReview(CreateReviewRequest reviewDto, int userId);
        Task UpdateReview(UpdateReviewRequest updatedReview, int userId);
        Task<GetReviewsResponse> GetReviewsAndProductos(int id);
    }

}
