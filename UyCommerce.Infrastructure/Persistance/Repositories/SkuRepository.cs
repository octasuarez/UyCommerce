using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Infrastructure.Persistance.Repositories
{
    public class SkuRepository : GenericRepository<Sku>, ISkuRepository
    {
        public SkuRepository(ShopContext context) : base(context)
        {
        }

        public async Task<bool> CheckSkuKeyExists(string skuKey)
        {
            return await _context.Skus.AnyAsync(s => s.Key.Equals(skuKey));
        }

        public async Task<string?> FindSkyKeyByParts(string[] keyParts)
        {
            var skus = _context.Skus.AsQueryable();
            foreach (var part in keyParts)
            {
                skus.Where(s => s.Key.Contains(part));
            }
            return await skus.Select(s => s.Key).FirstOrDefaultAsync();
        }

        public async Task<Sku?> GetSkuByKeyAsync(string key)
        {
            return await _context.Skus
                .AsNoTracking()
                .Include(s => s.AtributosValores)
                .Include(s => s.Producto).ThenInclude(p => p!.Reviews)!.ThenInclude(r => r.Usuario)
                .Include(s => s.Producto).ThenInclude(p => p!.Preguntas).ThenInclude(pr => pr.Usuario)
                .Include(s => s.Producto).ThenInclude(p => p!.Preguntas).ThenInclude(pr => pr.Respuesta)
                .Include(s => s.Producto).ThenInclude(p => p!.Reviews)
                .Include(s => s.Producto).ThenInclude(p => p!.Marca)
                .Include(s => s.Producto).ThenInclude(p => p!.Categoria).ThenInclude(c => c!.CategoriaPadre)
                .Include(s => s.Imagenes)
                .FirstOrDefaultAsync(s => s.Key.ToLower().Contains(key.ToLower()));

        }

        public async Task<List<Sku>> GetSkusByProductoIdAsync(int productoId)
        {
            return await _context.Skus
                    .AsNoTracking()
                    .Include(s => s.AtributosValores)!.ThenInclude(a => a.Atributo)
                    .Where(s => s.ProductoId == productoId)
                    .ToListAsync();

        }
    }
}
