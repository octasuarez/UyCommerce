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

        public async Task<string?> FindSkuKeyByKey(string skuKey)
        {
            return await _context.Skus.Select(s => s.Key).FirstOrDefaultAsync(k => k.StartsWith(skuKey));
        }

        public async Task<string?> FindSkuKeyByParts(string[] keyParts)
        {
            var skus = await _context.Skus.Where(s => keyParts.All(k => s.Key.Contains(k))).ToListAsync();
            return skus.FirstOrDefault().Key;
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

        public async Task<List<Sku>> GetSkusByCategoriaId(int categoriaId)
        {
            return await _context.Skus
                .AsNoTracking()
                .Include(s => s.Imagenes)
                .Include(s => s.Producto).ThenInclude(p => p.Reviews)
                .Where(s => s.Producto.Categoria.Id == categoriaId).ToListAsync();
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
