using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Infrastructure.Persistance.Repositories
{
    public class CategoriaRepository : GenericRepository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(ShopContext context) : base(context)
        {}

        public async Task<List<Categoria>> GetCategoriasInicio()
        {
            return await _context.Categorias
                .AsNoTracking()
                .Where(c => c.MostrarEnInicio == true).ToListAsync();
        }
        
    }
}
