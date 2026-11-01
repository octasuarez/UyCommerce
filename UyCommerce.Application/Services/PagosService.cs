using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Application.Contracts.Persistance.Services;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Application.DTOs.ExternalServices.Paypal;
using UyCommerce.Domain.Entities;
using UyCommerce.Domain.Exceptions;

namespace UyCommerce.Application.Services
{
    public class PagosService : IPagosService
    {
        public ICarritoRepository _carritoRepository { get; }
        public IPaypalService _paypalService { get; }
        public IUsuarioRepository _usuarioRepository { get; }
        public IProductoRepository _productoRepository { get; }
        public IOrdenRepository _ordenRepository { get; }

        public PagosService(ICarritoRepository carritoService
            , IPaypalService paypalService
            , IUsuarioRepository usuarioRepository
            , IProductoRepository productoRepository
            , IOrdenRepository ordenRepository)
        {
            _carritoRepository = carritoService;
            _paypalService = paypalService;
            _usuarioRepository = usuarioRepository;
            _productoRepository = productoRepository;
            _ordenRepository = ordenRepository;
        }
        public async Task<CaptureOrderResponse> CaptureOrderPaypal(string ordenId, int userId)
        {
            var response = await _paypalService.CaptureOrder(ordenId)
                    ?? throw new Exception("No se concretar la orden");
            var usuario = await _usuarioRepository.GetByIdAsync(userId)
                    ?? throw new NotFoundException(nameof(Usuario),userId);
            var orden = CreateOrdenFromPaypalResponse(response, usuario);
            await _productoRepository.UpdateStock(orden.Productos.ToList());
            var carrito = await _carritoRepository.GetCarritoByUserIdAsync(userId);
            await _carritoRepository.DeleteProductosCarritoByCarritoIdAsync(carrito.Id);
            await _ordenRepository.CreateAsync(orden);
            return response;
        }

        public async Task<CreateOrderResponse> CreateOrderPaypal(int userId)
        {
            var carrito = await _carritoRepository.GetCarritoByUserIdAsync(userId);
            if (!carrito.Productos.Any())
            {
                throw new Exception("No tiene productos en su carrito");
            }
            var total = carrito.GetTotal().ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            var response = await _paypalService.CreateOrder(CreateItems(carrito.Productos), total)
                ?? throw new Exception("No se pudo crear la orden de paypal.");
            return response;
        }

        private static List<Item> CreateItems(ICollection<ProductoCarrito> products)
        {
            var items = new List<Item>();

            foreach (var p in products)
            {
                items.Add(new Item
                {
                    name = p.Sku!.Key,
                    quantity = p!.Cantidad.ToString(),
                    unit_amount = new Amount
                    {
                        currency_code = "USD",
                        value = p.Sku?.Precio.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)
                    },
                    sku = p!.SkuId.ToString()
                });
            }

            return items;

        }

        private static Orden CreateOrdenFromPaypalResponse(CaptureOrderResponse response, Usuario usuario)
        {

            Orden orden = new()
            {
                PaypalOrderId = response.id!,
                Usuario = usuario,
                FechaDeCompra = DateTime.Now,
                Estado = new() { Id = 2 },
                NombreComprador = usuario?.Nombre,
                MetodoDePago = response.payment_source!.card != null ? "Tarjeta" : "Paypal",
                Total = double.Parse(response.purchase_units![0].amount!.value!),
                Direccion = JsonSerializer.Deserialize<OrdenDireccion>(JsonSerializer.Serialize(response.purchase_units[0].shipping?.address)),
                Productos = new List<ProductoOrden>()
            };

            foreach (var item in response.purchase_units![0].items!)
            {
                orden.Productos?.Add(new ProductoOrden
                {
                    SkuId = int.Parse(item.sku!),
                    OrdenId = orden.Id,
                    Cantidad = int.Parse(item.quantity!),
                    Precio = double.Parse(item.unit_amount!.value!),
                    Nombre = item.name,
                });
            }

            return orden;
        }
    }
}
