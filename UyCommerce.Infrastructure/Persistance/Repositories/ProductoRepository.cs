using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Common.Enums;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Application.DTOs;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Infrastructure.Persistance.Repositories
{
    public class ProductoRepository : GenericRepository<Producto>, IProductoRepository
    {
        public ProductoRepository(ShopContext context) : base(context) { }

        public async Task<List<Producto>> GetFeaturedProductsAsync()
        {
            return await _context.Productos
            .Include(p => p.Reviews)
            .Include(p => p.Skus)!.ThenInclude(s => s.Imagenes)
            .Where(p => p.Featured == true)
            .ToListAsync();
        }

        public async Task<Producto?> GetProductoByNombreAsync(string nombre)
        {
            return await _context.Productos
                .FirstOrDefaultAsync(p => p.Nombre!.Equals(nombre, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<Producto?> GetProductoCompleteByIdAsync(int id)
        {
            return await GetQueryProductosComplete()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Producto?> GetProductoCompleteByIdNoTrackingAsync(int id)
        {
            return await GetQueryProductosComplete()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<GetProductosDto> GetProductosByQueryAsync(string query, ProductsOrderBy orderBy, int pag, int qty)
        {
            GetProductosDto result = new();
            var items = GetQueryProductosComplete()
                .AsNoTracking()
                .Where(p => p.Nombre.ToLower().StartsWith(query.ToLower()))
                .SelectMany(p => p.Skus)
                .Include(s => s.Producto).ThenInclude(p => p.Reviews);

            result.TotalItems = items.Count();

            switch (orderBy)
            {
                case (ProductsOrderBy.PriceDesc):
                    items.OrderByDescending(s => s.Precio);
                    break;
                case (ProductsOrderBy.PriceAsc):
                    items.OrderBy(s => s.Precio);
                    break;
                case (ProductsOrderBy.RatingDesc):
                    items.OrderByDescending(s => s.Producto!.GetPuntuacionPromedio());
                    break;
                default:
                    break;
            }
            result.Skus = await items.Skip(pag * qty).Take(qty).ToListAsync();
            return result;
        }

        public async Task<List<Producto>> GetProductosByQueryNameAsync(string query)
        {
            return await _context.Productos
                .AsNoTracking()
                .Include(p => p.Imagenes)
                .Where(p => p.Nombre.ToLower().Contains(query.ToLower()))
                .ToListAsync();
        }

        public async Task<List<Producto>> GetProductosCompleteAsync()
        {
            return await GetQueryProductosComplete()
                .AsNoTracking()
                .ToListAsync();
        }

        private IQueryable<Producto> GetQueryProductosComplete()
        {
            return _context.Productos
                .Include(p => p.Imagenes)
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.Preguntas)
                .Include(p => p.Reviews)
                .Include(p => p.Skus)!.ThenInclude(s => s.Imagenes)
                .Include(p => p.Skus)!.ThenInclude(s => s.AtributosValores)!.ThenInclude(a => a.Atributo)
                .AsQueryable();
        }
    }
}
