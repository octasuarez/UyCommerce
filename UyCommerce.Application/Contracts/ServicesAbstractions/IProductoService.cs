using UyCommerce.Application.Common.Enums;
using UyCommerce.Application.DTOs.Productos;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IProductoService
    {
        Task<List<Producto>> GetFeaturedProducts();
        Task<List<Producto>> GetProductosComplete();
        Task AddProducto(AddProductoRequest addProductoDto);
        Task<Producto?> GetProductoCompleteByIdNoTracking(int id);
        Task<GetProductosResponse> GetProductosByQuery(string query, ProductsOrderBy orderBy, int pag, int qty, int? userId);
        Task<List<GetProductosByQueryNameResponse>> GetProductosByQueryName(string query);
        Task AddSkuAProducto(AddSkuAProductoRequest addSkuAProductoDto);
        Task DeleteSku(int skuId);
        Task FeatureProduct(int id, bool feature);
        Task<GetSkuByKeyResponse> GetSkuByKey(string key, int? userId);
        Task<string> FindSkuKeyMatch(string key);
        Task DeleteProducto(int id);
        Task<GetProductosResponse> GetProductosByCategoria(string categoria, string[]? filtros, int? usuarioId, int orden, int pag, int qty);
    }
}
