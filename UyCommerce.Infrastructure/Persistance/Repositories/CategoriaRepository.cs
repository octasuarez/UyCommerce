using Microsoft.EntityFrameworkCore;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Infrastructure.Persistance.Repositories
{
    public class CategoriaRepository : GenericRepository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(ShopContext context) : base(context)
        {}

        public async Task<Categoria?> GetCategoriaByIdCompleteAsync(int id)
        {
            return await _context.Categorias
                .Include(c => c.Atributos)
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Categoria?> GetCategoriaByNameAsync(string name)
        {
            return await _context.Categorias
                .Include(c => c.SubCategorias)
                .Include(c => c.CategoriaPadre).ThenInclude(p => p.SubCategorias)
                .FirstOrDefaultAsync(c => c.Nombre.ToLower() == name.ToLower());
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

        public async Task<List<Categoria>> GetCategoriasPadreAsync()
        {
            return await _context.Categorias
                .AsNoTracking()
                .Where(c => c.CategoriaPadre == null)
                .ToListAsync();
        }
        
    }
}
