using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Infrastructure.Persistance.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(ShopContext context): base(context)
        {
            
        }

        public async Task<List<Review>> GetReviewsAsync()
        {
            return await _context.Reviews
                .AsNoTracking()
                .Include(r => r.Usuario)
                .Include(r => r.Producto).ThenInclude(p => p!.Imagenes)
                .ToListAsync();
        }

        public async Task<List<Review>> GetReviewsByUsuarioId(int id)
        {
            return await _context.Reviews
                .AsNoTracking()
                .Include(r => r.Producto).ThenInclude(p => p.Skus)
                .Include(r => r.Producto).ThenInclude(p => p.Imagenes)
                .Where(r => r.UsuarioId == id)
                .ToListAsync();
        }
    }
}
