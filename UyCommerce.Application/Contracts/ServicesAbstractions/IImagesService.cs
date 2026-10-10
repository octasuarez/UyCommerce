using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IImagesService
    {
        Task<List<CarouselImage>> GetCarouselImagesAsync();
    }
}
