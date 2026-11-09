using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.DTOs;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IReviewService
    {
        Task<List<Review>> GetReview();
        Task DeleteReview(int id);
        Task CreateReview(ReviewDTO reviewDto, int userId);
        Task UpdateReview(UpdateReviewDTO updatedReview, int userId);
        Task<GetReviewsDto> GetReviewsAndProductos(int id);
    }

}
