using UyCommerce.Application.Common.Enums;
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
        Task<GetProductosDto> GetProductosByQueryAsync(string query, ProductsOrderBy orderBy, int pag, int qty, int? userId);
        Task<List<GetProductosByQueryNameDto>> GetProductosByQueryNameAsync(string query);
        Task AddSkuAProductoAsync(AddSkuAProductoDto addSkuAProductoDto);
        Task DeleteSkuAsync(int skuId);
        Task FeatureProductAsync(int id, bool feature);
        Task<GetSkuByKeyDto> GetSkuByKeyAsync(string key, int? userId);
        Task<string> FindSkuKeyMatchAsync(string key);
        Task DeleteProductoAsync(int id);
        Task<GetProductosDto> GetProductosByCategoriaAsync(string categoria, string[]? filtros, int? usuarioId, int orden, int pag, int qty);
    }
}
