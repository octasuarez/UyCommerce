using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IProductoService
    {
        Task<List<Producto>> GetFeaturedProductsAsync();
        Task<List<Producto>> GetProductosCompleteAsync();
    }
}
