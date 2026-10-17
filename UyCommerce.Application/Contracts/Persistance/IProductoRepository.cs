using UyCommerce.Application.Common.Enums;
using UyCommerce.Application.Common.Models;
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
        Task<GetProductsByQuery> GetProductosByQueryAsync(string query, ProductsOrderBy orderBy, int pag, int qty);
        Task<List<Producto>> GetProductosByQueryNameAsync(string query);
    }
}
