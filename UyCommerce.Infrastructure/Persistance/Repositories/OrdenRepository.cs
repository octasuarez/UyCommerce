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
        public async Task<List<Orden>> GetOrdenesPorFecha(DateTime startDate, DateTime endDate)
        {
            return await _context.Ordenes
                .Include(o => o.Productos)!.ThenInclude(p => p.Sku).ThenInclude(s => s.Imagenes)
                .Where(o => o.FechaDeCompra > startDate && o.FechaDeCompra < endDate).ToListAsync();

        }
    }
}
