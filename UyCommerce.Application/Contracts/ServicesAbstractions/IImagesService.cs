using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IImagesService
    {
        Task DeleteCarouselImage(int id);
        Task<List<CarouselImage>> GetCarouselImagesAsync();
        Task AddCarouselImageAsync(string name, Stream stream);
    }
}
