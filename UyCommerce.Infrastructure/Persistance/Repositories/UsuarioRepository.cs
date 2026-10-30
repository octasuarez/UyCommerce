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

        public async Task<Usuario?> GetUsuarioByEmailAsync(string email)
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Carrito).ThenInclude(c => c.Productos)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email);
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

        public async Task<List<Rol>> GetRolesUsuarios()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Rol?> GetRolByNombre(string nombre)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == nombre);
        }

        public async Task<Codigo?> GetCodigoRecuperacionUsuarioAsync(int id)
        {
            return await _context.Codigos.FirstOrDefaultAsync(c => c.UsuarioId == id);
        }

        public async Task<int> CreateCodigoRecuperacion(Codigo codigo)
        {
            await _context.Codigos.AddAsync(codigo);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateCodigoRecuperacion(Codigo codigo)
        {
            _context.Codigos.Update(codigo);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Codigo?> GetCodigoRecuperacionByCodigoAsync(string codigo)
        {
            return await _context.Codigos.FirstOrDefaultAsync(c => c.CodigoRecuperacion == codigo);
        }
    }
}
