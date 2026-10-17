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
        Task<bool> CheckSkuKeyExists(string skuKey);
        Task<string?> FindSkyKeyByParts(string[] keyParts);
    }
}
