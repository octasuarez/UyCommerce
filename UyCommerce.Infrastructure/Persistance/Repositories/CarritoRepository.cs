using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Infrastructure.Persistance.Repositories
{
    public class CarritoRepository : GenericRepository<Carrito>, ICarritoRepository
    {
        public CarritoRepository(ShopContext context): base(context)
        {}
        public async Task<Carrito?> GetCarritoByUserIdAsync(int userId)
        {
            return await _context.Carritos
                        .Include(c => c.Productos)!
                        .ThenInclude(pC => pC.Sku)
                        .FirstOrDefaultAsync(c => c.UsuarioId == userId);
        }

    }
}
