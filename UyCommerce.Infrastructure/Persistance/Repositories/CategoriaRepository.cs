using Microsoft.EntityFrameworkCore;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Infrastructure.Persistance.Repositories
{
    public class CategoriaRepository : GenericRepository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(ShopContext context) : base(context)
        {}

        public async Task<Categoria?> GetCategoriaByNameAsync(string name)
        {
            return await _context.Categorias.Where(c => c.Nombre == name).FirstOrDefaultAsync();
        }

        public async Task<List<Categoria>> GetCategoriasCompleteAsync()
        {
            return await _context.Categorias
                .Include(c => c.Atributos)
                .Include(c => c.Productos)
                .ToListAsync();
        }
        public async Task<List<Categoria>> GetCategoriasInicioAsync()
        {
            return await _context.Categorias
                .AsNoTracking()
                .Where(c => c.MostrarEnInicio == true).ToListAsync();
        }
        
    }
}
