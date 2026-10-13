using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Infrastructure.Persistance.Repositories
{
    public class AtributoRepository : GenericRepository<Atributo>, IAtributoRepository
    {
        public AtributoRepository(ShopContext context) : base(context)
        {

        }
        public async Task<List<Atributo>> GetAtributosAsync()
        {
            return await _context.Atributos
                .AsNoTracking()
                .Include(a => a.Categorias)
                .Include(a => a.Productos)
                .Include(a => a.Valores)
                .ToListAsync();
        }

        public async Task<List<Atributo>?> GetAtributosByCategoriaAsync(Categoria categoria)
        {
            return await _context.Atributos.Where(a => a.Categorias != null && a.Categorias.Contains(categoria)).ToListAsync();
        }
    }
}
