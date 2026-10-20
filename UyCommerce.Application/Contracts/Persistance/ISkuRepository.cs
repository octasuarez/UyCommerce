using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.Persistance
{
    public interface ISkuRepository : IGenericRepository<Sku>
    {
        Task<Sku?> GetSkuByKeyAsync(string key);
        Task<List<Sku>> GetSkusByProductoIdAsync(int productoId);
        Task<string?> FindSkuKeyByKey(string skuKey);
        Task<string?> FindSkuKeyByParts(string[] keyParts);
        Task<List<Sku>> GetSkusByCategoriaId(int categoriaId);
    }
}
