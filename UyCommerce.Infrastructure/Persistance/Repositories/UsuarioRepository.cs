using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Infrastructure.Persistance.Repositories
{
    public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(ShopContext context):base(context)
        {
        }

        public async Task<Usuario?> GetUsuarioAndFavoritos(int id)
        {
            return await _context.Usuarios
                .Include(u => u.Favoritos)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<Usuario>> GetUsuariosClientesAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Carrito).ThenInclude(c => c.Productos)
                .Include(u => u.Preguntas)
                .Include(u => u.Ordenes)
                .Include(u => u.Reviews)
                .Where(u => u.Rol!.Nombre == "User")
                .ToListAsync();
        }
    }
}
