using UyCommerce.Application.DTOs.Carritos;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface ICarritoService
    {
        Task<GetCarritoCompleteByUserIdResponse> GetCarritoCompleteByUserId(int userId);
        Task<int> AddProductoAlCarrito(int userId, int skuId, int quantity);
        Task DeleteProductoCarrito(int userId, int skuId);
    }
}
