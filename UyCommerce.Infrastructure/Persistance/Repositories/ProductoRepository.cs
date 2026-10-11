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

        public async Task<List<Producto>> GetProductosCompleteAsync()
        {
            return await _context.Productos
                .AsNoTracking()
                .Include(p => p.Imagenes)
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.Preguntas)
                .Include(p => p.Reviews)
                .Include(p => p.Skus)!.ThenInclude(s => s.Imagenes)
                .Include(p => p.Skus)!.ThenInclude(s => s.AtributosValores)!.ThenInclude(a => a.Atributo)
                .ToListAsync();
        }
    }
}
