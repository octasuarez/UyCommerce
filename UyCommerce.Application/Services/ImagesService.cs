using Mapster;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
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
        private readonly string _carouselImagePath;
        public ImagesService(ICarouselImageRepository carouselImageRepository, IFileService fileService)
        {
            _carouselImageRepository = carouselImageRepository;
            _fileService = fileService;
            _carouselImagePath = "Imagenes/Carousel";
        }

        #region carouselImages
        public async Task AddCarouselImageAsync(string name, Stream stream)
        {
            if (string.IsNullOrEmpty(name) || stream.Length <= 0)
            {
                throw new ArgumentNullException(nameof(name));
            }
            await this.SaveImageFile(name, stream, this._carouselImagePath);
            CarouselImage newImage = new() { Name = name };
            await _carouselImageRepository.CreateAsync(newImage);
        }

        public async Task DeleteCarouselImage(int id)
        {
            var carImage = await _carouselImageRepository.GetByIdAsync(id) ?? throw new NotFoundException(nameof(CarouselImage), id);
            await _carouselImageRepository.DeleteAsync(carImage);
            _fileService.DeleteFile([carImage.Name], this._carouselImagePath);
        }

        public async Task<CarouselImage> GetCarouselImageByIdAsync(int id)
        {
            return await _carouselImageRepository.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(CarouselImage), id);
        }

        public async Task<List<CarouselImage>> GetCarouselImagesAsync()
        {
            return await _carouselImageRepository.GetAllAsync();
        }

        public async Task UpdateCarouselImageAsync(int id, string imgName, Stream stream)
        {
            var imageDb = await GetCarouselImageByIdAsync(id);
            string imageToDelete = imageDb.Name.ToString();
            if (stream.Length <= 0) throw new Exception($"Imagen {imgName} invalida.");
            await this.SaveImageFile(imgName, stream, this._carouselImagePath);
            imageDb.Name = imgName;
            await _carouselImageRepository.UpdateAsync(imageDb);
            _fileService.DeleteFile([imageToDelete], this._carouselImagePath);
        }

        #endregion carouselImages

        public async Task SaveImageFile(string imgName, Stream stream, string path)
        {
            if (!await _fileService.SaveFile(imgName, stream, path))
            {
                throw new ArgumentOutOfRangeException($"No se pudo guardar la imagen {imgName}");
            }
        }

    }
}
