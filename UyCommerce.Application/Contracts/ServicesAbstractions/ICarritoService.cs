using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface ICarritoService
    {
        Task<Carrito> GetCarritoByUserIdAsync(int userId);
        Task<int> AddProductoAlCarritoAsync(int userId, int skuId, int quantity);
        Task DeleteProductoCarritoAsync(int userId, int skuId);
    }
}
