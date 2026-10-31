using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.Persistance
{
    public interface ICarritoRepository : IGenericRepository<Carrito>
    {
        Task<Carrito?> GetCarritoByUserIdAsync(int userId);
        Task<Carrito?> GetCarritoCompleteByUserIdAsync(int userId);
        Task<List<ProductoCarrito>> GetProductosCarritoByProductoIdAsync(int productoId);
        Task DeleteProductosCarritoAsync(List<ProductoCarrito> productosCarrito);
    }
}
