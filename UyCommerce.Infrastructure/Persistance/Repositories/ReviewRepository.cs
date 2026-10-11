using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Infrastructure.Persistance.Repositories
{
    public class ReviewRepository : GenericRepository<Review> ,IReviewRepository
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
    }
}
