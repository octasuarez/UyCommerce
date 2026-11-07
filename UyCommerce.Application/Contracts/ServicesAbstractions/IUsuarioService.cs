using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.DTOs;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IUsuarioService
    {
        Task<Usuario?> GetUsuarioByEmail(string email);
        Task<Usuario> GetUsuarioFavoritosByIdAsync(int id);
        Task<List<Usuario>> GetUsuariosClientesAsync();
        Task<Usuario> GetUsuarioById(int id);
        Task<List<FavoritosDto>> GetFavoritosByUsuarioId(int id);
        Task<string> AddToFavoritos(int skuId, int userId);
        Task<GetReviewsDto> GetReviewsAndProductos(int id);
        Task CreateReview(ReviewDTO reviewDto, int userId);
        Task UpdateReview(UpdateReviewDTO updatedReview, int userId);
    }
}
