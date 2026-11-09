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

        public async Task AddMarca(string nombre)
        {
            var marcaDb = await _marcaRepository.GetMarcaByNombre(nombre);
            if (marcaDb is not null)
                throw new Exception($"{nameof(Marca)} {nombre} ya existe.");

            Marca newMarca = new() { Nombre = nombre };
            await _marcaRepository.CreateAsync(newMarca);
        }

        public async Task DeleteMarca(int id)
        {
            var marca = await _marcaRepository.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(Marca), id);
            await _marcaRepository.DeleteAsync(marca);
        }

        public async Task<Marca> GetMarcaAndProductosById(int id)
        {
            return await _marcaRepository.GetMarcaAndProductosAsync(id);
        }

        public async Task<Marca> GetMarcaById(int id)
        {
            return await _marcaRepository.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(Marca), id);
        }

        public async Task<List<Marca>> GetMarcas()
        {
            return await _marcaRepository.GetAllAsync();
        }

        public async Task<List<Marca>> GetMarcasWithProductos()
        {
            return await _marcaRepository.GetMarcasWithProductosAsync();
        }
    }
}
