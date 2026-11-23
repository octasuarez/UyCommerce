using UyCommerce.Application.DTOs.Carritos;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface ICarritoService
    {
        Task<Carrito> GetCarritoByUserId(int userId);
        Task<GetCarritoCompleteByUserIdResponse> GetCarritoCompleteByUserId(int userId);
        Task<int> AddProductoAlCarrito(int userId, int skuId, int quantity);
        Task DeleteProductoCarrito(int userId, int skuId);
        Task<List<ProductoCarrito>> GetProductosCarritoByProductoId(int productoId);
        Task DeleteProductosCarrito(List<ProductoCarrito> productosCarrito);
    }
}
