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
        private readonly ShopContext _context;
        public UsuarioRepository(ShopContext context):base(context)
        {
            _context = context;
        }

        public async Task<Usuario> GetUsuarioAndFavoritos(int id)
        {
            return await _context.Usuarios.Include(u => u.Favoritos).FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
