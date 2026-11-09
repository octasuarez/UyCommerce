using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface ICarritoService
    {
        Task<Carrito> GetCarritoByUserId(int userId);
        Task<Carrito> GetCarritoCompleteByUserId(int userId);
        Task<int> AddProductoAlCarrito(int userId, int skuId, int quantity);
        Task DeleteProductoCarrito(int userId, int skuId);
        Task<List<ProductoCarrito>> GetProductosCarritoByProductoId(int productoId);
        Task DeleteProductosCarrito(List<ProductoCarrito> productosCarrito);
    }
}
