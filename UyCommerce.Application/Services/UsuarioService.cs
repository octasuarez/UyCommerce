using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Application.DTOs;
using UyCommerce.Domain.Entities;
using UyCommerce.Domain.Exceptions;

namespace UyCommerce.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly ISkuRepository _skuRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IOrdenRepository _ordenRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository
            , IProductoRepository productoRepository
            , ISkuRepository skuRepository
            , IReviewRepository reviewRepository
            , IOrdenRepository ordenRepository)
        {
            _usuarioRepository = usuarioRepository;
            this._productoRepository = productoRepository;
            this._skuRepository = skuRepository;
            this._reviewRepository = reviewRepository;
            this._ordenRepository = ordenRepository;
        }

        public async Task<List<Usuario>> GetUsuariosClientesAsync()
        {
            return await _usuarioRepository.GetUsuariosClientesAsync();
        }

        public async Task<Usuario> GetUsuarioFavoritosByIdAsync(int id)
        {
            var usuario = await _usuarioRepository.GetUsuarioAndFavoritos(id);
            if (usuario is null)
            {
                throw new NotFoundException(nameof(Usuario), id);
            }

            return usuario;
        }

        public async Task<Usuario?> GetUsuarioByEmail(string email)
        {
            return await _usuarioRepository.GetUsuarioByEmailAsync(email);
        }

        public async Task<Usuario> GetUsuarioById(int id)
        {
            var usuario = await _usuarioRepository.GetUsuarioByIdNoTracking(id)
                ?? throw new NotFoundException(nameof(Usuario), id);
            return usuario;
        }

        public async Task<List<FavoritosDto>> GetFavoritosByUsuarioId(int id)
        {
            var usuario = await GetUsuarioById(id);
            List<FavoritosDto> favoritos = [.. usuario.Favoritos.Select(f => new FavoritosDto
            {
                Id = f.Id,
                Key = f.Key,
                Imagenes = f.Imagenes ?? [],
                Nombre = f.Nombre,
                Precio = f.Precio,
                PrecioAnterior = f.PrecioAnterior,
                Stock = f.Stock,
                AtributosValores = f.AtributosValores ?? [],
                IsInCart = usuario.Carrito.Productos?.FirstOrDefault(p => p.SkuId == f.Id) is not null
            })];
            return favoritos;
        }

        public async Task<string> AddToFavoritos(int skuId, int userId)
        {
            var usuario = await _usuarioRepository.GetUsuarioAndFavoritos(userId)
                ?? throw new NotFoundException(nameof(Usuario), userId);
            var sku = await _skuRepository.GetByIdAsync(skuId)
                ?? throw new NotFoundException(nameof(Sku), skuId);
            if (!usuario.Favoritos.Remove(sku)){ usuario.Favoritos.Add(sku); }
            await _usuarioRepository.UpdateAsync(usuario);
            return (usuario.Favoritos.Contains(sku) ? "Se agrego a favoritos" : "Se elimino de favoritos");
        }

        public async Task<GetReviewsDto> GetReviewsProductos(int id)
        {
            GetReviewsDto result = new();

            var reviews = await _reviewRepository.GetReviewsByUsuarioId(id);
            var ordenes = await _ordenRepository.GetOrdenesByUsuarioId(id);
            var reviewsSkus = reviews.SelectMany(r => r.Producto.Skus).Select(s => s.Id).ToList();
            List<Orden> ordenesSinReview = [];
            foreach(var orden in ordenes)
            {
                //if(reviewsSkus.Contains(orden.Productos))
            }

            throw new NotImplementedException();
        }
    }
}
