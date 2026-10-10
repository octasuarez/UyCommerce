using Mapster;
using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Services
{
    public class ImagesService : IImagesService
    {
        private readonly ICarouselImageRepository _carouselImageRepository;

        public ImagesService(ICarouselImageRepository carouselImageRepository)
        {
            _carouselImageRepository = carouselImageRepository;
        }

        public async Task<List<CarouselImage>> GetCarouselImagesAsync()
        {
            return await _carouselImageRepository.GetAllAsync();
        }
    }
}
