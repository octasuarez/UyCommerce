using Mapster;
using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Application.Contracts.Persistance.Services;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Domain.Entities;
using UyCommerce.Domain.Exceptions;

namespace UyCommerce.Application.Services
{
    public class ImagesService : IImagesService
    {
        private readonly ICarouselImageRepository _carouselImageRepository;
        private readonly IFileService _fileService;

        public ImagesService(ICarouselImageRepository carouselImageRepository, IFileService fileService)
        {
            _carouselImageRepository = carouselImageRepository;
            _fileService = fileService;
        }

        public async Task AddCarouselImageAsync(string name, Stream stream)
        {
            if (string.IsNullOrEmpty(name) || stream.Length <= 0)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (!await _fileService.SaveFile(name, stream, "Imagenes/Carousel"))
            {
                throw new ArgumentOutOfRangeException(nameof(name));
            }

            CarouselImage newImage = new CarouselImage() { Name = name };
            await _carouselImageRepository.CreateAsync(newImage);
        }

        public async Task DeleteCarouselImage(int id)
        {
            var carImage = await _carouselImageRepository.GetByIdAsync(id) ?? throw new NotFoundException(nameof(CarouselImage), id);
            await _carouselImageRepository.DeleteAsync(carImage);
            _fileService.DeleteFile([carImage.Name], "Imagenes/Carousel");
        }

        public async Task<List<CarouselImage>> GetCarouselImagesAsync()
        {
            return await _carouselImageRepository.GetAllAsync();
        }
    }
}
