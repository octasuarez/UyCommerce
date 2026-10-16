using UyCommerce.Application.Common.Enums;
using UyCommerce.Application.Common.Models;
using UyCommerce.Application.DTOs;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IProductoService
    {
        Task<List<Producto>> GetFeaturedProductsAsync();
        Task<List<Producto>> GetProductosCompleteAsync();
        Task AddProductoAsync(AddProductoDto addProductoDto);
        Task<Producto?> GetProductoCompleteByIdAsync(int id);
        Task<GetProductsByQuery> GetProductosByQuery(string query, ProductsOrderBy orderBy, int pag, int qty);
    }
}
