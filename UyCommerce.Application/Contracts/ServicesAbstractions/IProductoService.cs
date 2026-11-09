using UyCommerce.Application.Common.Enums;
using UyCommerce.Application.DTOs;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IProductoService
    {
        Task<List<Producto>> GetFeaturedProducts();
        Task<List<Producto>> GetProductosComplete();
        Task AddProductoAsync(AddProductoDto addProductoDto);
        Task<Producto?> GetProductoCompleteByIdNoTracking(int id);
        Task<GetProductosDto> GetProductosByQuery(string query, ProductsOrderBy orderBy, int pag, int qty, int? userId);
        Task<List<GetProductosByQueryNameDto>> GetProductosByQueryName(string query);
        Task AddSkuAProducto(AddSkuAProductoDto addSkuAProductoDto);
        Task DeleteSku(int skuId);
        Task FeatureProduct(int id, bool feature);
        Task<GetSkuByKeyDto> GetSkuByKey(string key, int? userId);
        Task<string> FindSkuKeyMatch(string key);
        Task DeleteProducto(int id);
        Task<GetProductosDto> GetProductosByCategoria(string categoria, string[]? filtros, int? usuarioId, int orden, int pag, int qty);
    }
}
