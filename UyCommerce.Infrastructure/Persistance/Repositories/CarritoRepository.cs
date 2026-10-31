using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Infrastructure.Persistance.Repositories
{
    public class CarritoRepository : GenericRepository<Carrito>, ICarritoRepository
    {
        public CarritoRepository(ShopContext context): base(context)
        {}

        public async Task DeleteProductosCarritoAsync(List<ProductoCarrito> productosCarrito)
        {
            _context.RemoveRange(productosCarrito);
            await _context.SaveChangesAsync();
        }

        public async Task<Carrito?> GetCarritoByUserIdAsync(int userId)
        {
            return await _context.Carritos
                        .Include(c => c.Productos)!
                        .ThenInclude(pC => pC.Sku)
                        .FirstOrDefaultAsync(c => c.UsuarioId == userId);
        }

        public async Task<Carrito?> GetCarritoCompleteByUserIdAsync(int userId)
        {
            return await _context.Carritos
                .AsNoTracking()
                .Include(c => c.Productos)!.ThenInclude(p => p.Sku).ThenInclude(s => s!.Imagenes)
                .Include(c => c.Productos)!.ThenInclude(p => p.Sku).ThenInclude(s => s!.Producto).ThenInclude(pr => pr!.Marca)
                .Include(c => c.Productos)!.ThenInclude(p => p.Sku).ThenInclude(s => s!.AtributosValores)!.ThenInclude(a => a.Atributo)
                .FirstOrDefault(c => c.UsuarioId.ToString() == userId);
        }

        public async Task<List<ProductoCarrito>> GetProductosCarritoByProductoIdAsync(int productoId)
        {
            return await _context.ProductosCarritos
                .Where(p => p.Sku.Producto.Id == productoId).ToListAsync();
        }
    }
}
