using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Domain.Entities;
using UyCommerce.Domain.Exceptions;

namespace UyCommerce.Application.Services
{
    public class CarritoService : ICarritoService
    {
        private readonly ICarritoRepository _carritoRepository;
        private readonly ISkuRepository _skuRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public CarritoService(ICarritoRepository carritoRepository, ISkuRepository skuRepository, IUsuarioRepository usuarioRepository)
        {
            _carritoRepository = carritoRepository;
            this._skuRepository = skuRepository;
            this._usuarioRepository = usuarioRepository;
        }

        public async Task<int> AddProductoAlCarritoAsync(int userId, int skuId, int quantity)
        {
            ProductoCarrito newProductoCarrito = new();
            int qty = quantity;

            var sku = await _skuRepository.GetByIdAsync(skuId)
                ?? throw new NotFoundException(nameof(Sku), skuId);

            var carrito = await _carritoRepository.GetCarritoByUserIdAsync(userId)
                ?? throw new NotFoundException(nameof(Carrito), userId);

            var productoCarrito = carrito.Productos.FirstOrDefault(p => p.SkuId == skuId);

            qty += (productoCarrito != null ? productoCarrito.Cantidad : 0);

            if (qty > sku.Stock)
                throw new ArgumentOutOfRangeException($"quantity {qty} is higher than available stock.");

            if (productoCarrito != null && qty <= 0)
                carrito.Productos.Remove(productoCarrito);

            if (productoCarrito != null && qty > 0)
                productoCarrito.Cantidad = qty;

            if (productoCarrito == null && qty > 0)
            {
                newProductoCarrito.SkuId = sku.Id;
                newProductoCarrito.Cantidad = qty;
                carrito.Productos.Add(newProductoCarrito);
            }
            await _carritoRepository.UpdateAsync(carrito);
            return qty;
        }

        public async Task DeleteProductoCarritoAsync(int userId, int skuId)
        {
            var carrito = await _carritoRepository.GetCarritoByUserIdAsync(userId)
                ?? throw new NotFoundException(nameof(Carrito), userId);

            var productoCarrito = carrito.Productos?.FirstOrDefault(p => p.SkuId == skuId)
                ?? throw new NotFoundException(nameof(Carrito), skuId);

            carrito.Productos.Remove(productoCarrito);
            await _carritoRepository.UpdateAsync(carrito);
        }

        public async Task DeleteProductosCarritoAsync(List<ProductoCarrito> productosCarrito)
        {
            await _carritoRepository.DeleteProductosCarritoAsync(productosCarrito);
        }

        public async Task<Carrito> GetCarritoByUserIdAsync(int userId)
        {
            var carrito = await _carritoRepository.GetCarritoByUserIdAsync(userId) 
                ?? throw new NotFoundException(nameof(Carrito), userId);
            return carrito;
        }

        public async Task<List<ProductoCarrito>> GetProductosCarritoByProductoIdAsync(int productoId)
        {
            return await _carritoRepository.GetProductosCarritoByProductoIdAsync(productoId);
        }
    }
}
