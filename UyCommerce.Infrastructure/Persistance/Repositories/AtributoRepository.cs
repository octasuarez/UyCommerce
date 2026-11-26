using Microsoft.EntityFrameworkCore;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Infrastructure.Persistance.Repositories
{
    public class AtributoRepository : GenericRepository<Atributo>, IAtributoRepository
    {
        public AtributoRepository(ShopContext context) : base(context)
        {}

        public async Task<Atributo?> GetAtributoByNameAsync(string name)
        {
            return await _context.Atributos
                .FirstOrDefaultAsync(a => a.Nombre.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<Atributo?> GetAtributoConValoresByIdAsync(int id)
        {
            return await _context.Atributos
                .Include(a => a.Valores)
                .FirstOrDefaultAsync(a => a.Id == id);
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

        public async Task<ICollection<Atributo>?> GetAtributosByCategoriaAsync(Categoria categoria)
        {
            var categoriaDb = await _context.Categorias
                .AsNoTracking()
                .Include(c => c.Atributos)
                .FirstOrDefaultAsync(c => c.Id == categoria.Id);

            return categoriaDb.Atributos;
        }
    }
}
