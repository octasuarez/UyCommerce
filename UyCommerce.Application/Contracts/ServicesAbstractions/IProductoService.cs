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
        Task<Producto?> GetProductoCompleteByIdNoTrackingAsync(int id);
        Task<GetProductsByQuery> GetProductosByQueryAsync(string query, ProductsOrderBy orderBy, int pag, int qty, int? userId);
        Task<List<GetProductosByQueryNameDto>> GetProductosByQueryNameAsync(string query);
        Task AddSkuAProductoAsync(AddSkuAProductoDto addSkuAProductoDto);
        Task DeleteSkuAsync(int skuId);
        Task FeatureProductAsync(int id, bool feature);
        Task<GetSkuByKeyDto> GetSkuByKeyAsync(string key, int? userId);
        Task<string> FindSkuKeyMatchAsync(string key);
    }
}
