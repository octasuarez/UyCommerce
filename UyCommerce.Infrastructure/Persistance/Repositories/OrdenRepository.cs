using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Infrastructure.Persistance.Repositories
{
    public class OrdenRepository : GenericRepository<Orden>, IOrdenRepository
    {
        public OrdenRepository(ShopContext context):base(context)
        {
        }

        public async Task<List<Orden>> GetOrdenesAsync()
        {
            return await GetOrdenesQuery().ToListAsync();
        }

        public async Task<List<Orden>> GetOrdenesByDateAsync(DateTime startDate, DateTime endDate)
        {
            return await GetOrdenesQuery()
                .Where(o => o.FechaDeCompra > startDate && o.FechaDeCompra < endDate)
                .ToListAsync();
        }

        public async Task<List<Orden>> GetOrdenesByProductoIdAsync(int productoId)
        {
            return await _context.Ordenes
                .AsNoTracking()
                .Where(o => o.Productos.Any(p => p.Sku.Producto.Id == productoId))
                .ToListAsync();
        }

        public async Task<List<Orden>> GetOrdenesUsuariosAsync()
        {
            return await _context.Ordenes
                .Include(o => o.Usuario)
                .ToListAsync();
        }

        private IQueryable<Orden> GetOrdenesQuery()
        {
            return  _context.Ordenes
                .AsNoTracking()
                .Include(o => o.Direccion)
                .Include(o => o.Productos)!
                .ThenInclude(p => p.Sku)
                .ThenInclude(s => s.Imagenes)
                .AsQueryable();
        }
    }
}
