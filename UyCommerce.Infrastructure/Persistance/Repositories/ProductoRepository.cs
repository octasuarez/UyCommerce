using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Infrastructure.Persistance.Repositories
{
    public class ProductoRepository : GenericRepository<Producto>, IProductoRepository
    {
        public ProductoRepository(ShopContext context) : base(context) { }
        public async Task<List<Producto>> GetFeaturedProductsAsync()
        {
            return await _context.Productos
            .Include(p => p.Reviews)
            .Include(p => p.Skus)!.ThenInclude(s => s.Imagenes)
            .Where(p => p.Featured == true)
            .ToListAsync();
        }
    }
}
