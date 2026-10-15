using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Domain.Entities;
using UyCommerce.Domain.Exceptions;

namespace UyCommerce.Application.Services
{
    public class MarcaService : IMarcaService
    {
        private readonly IMarcaRepository _marcaRepository;

        public MarcaService(IMarcaRepository marcaRepository)
        {
            _marcaRepository = marcaRepository;
        }

        public async Task AddMarcaAsync(string nombre)
        {
            var marcaDb = await _marcaRepository.GetMarcaByNombre(nombre);
            if (marcaDb is not null)
                throw new Exception($"{nameof(Marca)} {nombre} ya existe.");

            Marca newMarca = new() { Nombre = nombre };
            await _marcaRepository.CreateAsync(newMarca);
        }

        public async Task DeleteMarcaAsync(int id)
        {
            var marca = await _marcaRepository.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(Marca), id);
            await _marcaRepository.DeleteAsync(marca);
        }

        public async Task<Marca> GetMarcaAndProductosByIdAsync(int id)
        {
            return await _marcaRepository.GetMarcaAndProductosAsync(id);
        }

        public async Task<Marca> GetMarcaByIdAsync(int id)
        {
            return await _marcaRepository.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(Marca), id);
        }

        public async Task<List<Marca>> GetMarcasAsync()
        {
            return await _marcaRepository.GetAllAsync();
        }

        public async Task<List<Marca>> GetMarcasWithProductosAsync()
        {
            return await _marcaRepository.GetMarcasWithProductosAsync();
        }
    }
}
