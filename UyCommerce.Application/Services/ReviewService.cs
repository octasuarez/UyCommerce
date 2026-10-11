using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Domain.Entities;
using UyCommerce.Domain.Exceptions;

namespace UyCommerce.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            this._reviewRepository = reviewRepository;
        }

        public async Task DeleteReviewAsync(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if(review is null)
            {
                throw new NotFoundException(nameof(Review),id);
            }
            await _reviewRepository.DeleteAsync(review);
        }

        public async Task<List<Review>> GetReviewAsync()
        {
            return await _reviewRepository.GetReviewsAsync();
        }
    }
}
