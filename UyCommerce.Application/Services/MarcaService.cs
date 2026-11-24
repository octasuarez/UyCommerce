using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Application.DTOs.Marcas;
using UyCommerce.Domain.Entities;
using UyCommerce.Domain.Exceptions;

namespace UyCommerce.Application.Services
{
    public class MarcaService : IMarcaService
    {
        private readonly IMarcaRepository _marcaRepository;
        private readonly IMapper _mapper;

        public MarcaService(IMarcaRepository marcaRepository, IMapper mapper)
        {
            _marcaRepository = marcaRepository;
            this._mapper = mapper;
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

        public async Task<GetMarcasResponse> GetMarcas()
        {
            return new GetMarcasResponse { Marcas = _mapper.Map<List<GetMarcasDto>>(await _marcaRepository.GetAllAsync()) };
        }

        public async Task<GetMarcasWithProductosResponse> GetMarcasWithProductos()
        {
            var marcas = await _marcaRepository.GetMarcasWithProductosAsync();
            return new GetMarcasWithProductosResponse() { Marcas = _mapper.Map<List<GetMarcasWithProductosMarcaDto>>(marcas) }; ;
        }
    }
}
