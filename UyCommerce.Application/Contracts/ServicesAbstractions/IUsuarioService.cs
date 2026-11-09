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
        Task<Usuario> GetUsuarioFavoritosById(int id);
        Task<List<Usuario>> GetUsuariosClientes();
        Task<Usuario> GetUsuarioById(int id);
        Task<List<FavoritosDto>> GetFavoritosByUsuarioId(int id);
        Task<string> AddToFavoritos(int skuId, int userId);
        
    }
}
