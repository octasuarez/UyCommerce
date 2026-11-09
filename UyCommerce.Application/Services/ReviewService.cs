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
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly IOrdenRepository _ordenRepository;

        public ReviewService(IReviewRepository reviewRepository
            , IUsuarioRepository usuarioRepository
            , IProductoRepository productoRepository
            , IOrdenRepository ordenRepository)
        {
            this._reviewRepository = reviewRepository;
            this._usuarioRepository = usuarioRepository;
            this._productoRepository = productoRepository;
            this._ordenRepository = ordenRepository;
        }

        public async Task DeleteReview(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if(review is null)
            {
                throw new NotFoundException(nameof(Review),id);
            }
            await _reviewRepository.DeleteAsync(review);
        }

        public async Task<List<Review>> GetReview()
        {
            return await _reviewRepository.GetReviewsAsync();
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
