using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Domain.Entities;
using UyCommerce.Domain.Exceptions;

namespace UyCommerce.Application.Services
{
    public class CarritoService : ICarritoService
    {
        private readonly ICarritoRepository _carritoRepository;

        public CarritoService(ICarritoRepository carritoRepository)
        {
            _carritoRepository = carritoRepository;
        }

        public async Task<Carrito> GetCarritoByUserIdAsync(int userId)
        {
            var carrito = await _carritoRepository.GetCarritoByUserIdAsync(userId) 
                ?? throw new NotFoundException(nameof(Carrito), userId);
            return carrito;
        }
    }
}
