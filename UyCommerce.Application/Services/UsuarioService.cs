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
            if (!usuario.Favoritos.Remove(sku)) { usuario.Favoritos.Add(sku); }
            await _usuarioRepository.UpdateAsync(usuario);
            return (usuario.Favoritos.Contains(sku) ? "Se agrego a favoritos" : "Se elimino de favoritos");
        }

        public async Task<GetReviewsDto> GetReviewsAndProductos(int id)
        {
            GetReviewsDto result = new()
            {
                Reviews = await _reviewRepository.GetReviewsByUsuarioId(id)
            };

            var productosReviews = result.Reviews.Count > 0 ? result.Reviews.Select(r => r.Producto).ToList() : [];
            var productosOrdenes = await _ordenRepository.GetProductosFromOrdenesByUsuarioId(id);
            foreach (var producto in productosOrdenes)
            {
                if (!productosReviews.Exists(pr => pr.Id == producto.Id))
                    result.Productos.Add(producto);
            }

            return result;
        }

        public async Task CreateReview(ReviewDTO reviewDto, int userId)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(userId)
                ?? throw new NotFoundException(nameof(Usuario), userId);
            var producto = await _productoRepository.GetByIdAsync(reviewDto.ProductoId)
                ?? throw new NotFoundException(nameof(Producto), reviewDto.ProductoId);
            Review review = new()
            {
                Puntuacion = reviewDto.Valoracion,
                Texto = reviewDto.Comentario,
                Producto = producto,
                ProductoId = reviewDto.ProductoId,
                Usuario = usuario,
                UsuarioId = usuario.Id,
            };

            usuario.Reviews.Add(review);
            await _usuarioRepository.UpdateAsync(usuario);
        }

        public async Task UpdateReview(UpdateReviewDTO updatedReview, int userId)
        {
            var reviewToUpdate = await _reviewRepository.GetByIdAsync(updatedReview.ReviewId)
                ?? throw new NotFoundException(nameof(Review), updatedReview.ReviewId);
            if (reviewToUpdate.UsuarioId != userId)
                throw new Exception($"Review no le pertence a usuario {userId}");

            reviewToUpdate.Texto = updatedReview.Comentario;
            reviewToUpdate.Puntuacion = updatedReview.Valoracion;
            await _reviewRepository.UpdateAsync(reviewToUpdate);
        }
    }
}
