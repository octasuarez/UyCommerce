using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IImagesService
    {
        Task AddCarouselImageAsync(string name, Stream stream);
        Task UpdateCarouselImageAsync(int id, string imgName, Stream stream);
        Task DeleteCarouselImage(int id);
        Task<List<CarouselImage>> GetCarouselImagesAsync();
        Task<CarouselImage> GetCarouselImageByIdAsync(int id);
    }
}
