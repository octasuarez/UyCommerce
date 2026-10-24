using UyCommerce.Application.Common.Enums;
using UyCommerce.Application.DTOs;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.Persistance
{
    public interface IProductoRepository : IGenericRepository<Producto>
    {
        Task<List<Producto>> GetFeaturedProductsAsync();
        Task<List<Producto>> GetProductosCompleteAsync();
        Task<Producto?> GetProductoCompleteByIdNoTrackingAsync(int id);
        Task<Producto?> GetProductoCompleteByIdAsync(int id);
        Task<Producto?> GetProductoByNombreAsync(string nombre);
        Task<GetProductosDto> GetProductosByQueryAsync(string query, ProductsOrderBy orderBy, int pag, int qty);
        Task<List<Producto>> GetProductosByQueryNameAsync(string query);
    }
}
