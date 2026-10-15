using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Infrastructure.Persistance.Repositories
{
    public class MarcaRepository : GenericRepository<Marca>, IMarcaRepository
    {
        public MarcaRepository(ShopContext context) : base(context)
        {
        }

        public async Task<Marca> GetMarcaAndProductosAsync(int id)
        {
            return await _context.Marcas
                .Include(m => m.Productos)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Marca?> GetMarcaByNombre(string nombre)
        {
            return await _context.Marcas
                .FirstOrDefaultAsync(m => m.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<List<Marca>> GetMarcasWithProductosAsync()
        {
            return await _context.Marcas
                .AsNoTracking()
                .Include(m => m.Productos).ToListAsync();
        }
    }
}
