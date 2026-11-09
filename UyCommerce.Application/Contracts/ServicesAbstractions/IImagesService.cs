using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IImagesService
    {
        Task AddCarouselImage(string name, Stream stream);
        Task UpdateCarouselImage(int id, string imgName, Stream stream);
        Task DeleteCarouselImage(int id);
        Task<List<CarouselImage>> GetCarouselImages();
        Task<CarouselImage> GetCarouselImageById(int id);
        Task SaveImageFile(string imgName, Stream stream, string path);
    }
}
