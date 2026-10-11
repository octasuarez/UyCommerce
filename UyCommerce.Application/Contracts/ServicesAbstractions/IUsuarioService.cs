using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IUsuarioService
    {
        Task<Usuario> GetUsuarioFavoritosByIdAsync(int id);
        Task<List<Usuario>> GetUsuariosClientesAsync();
    }
}
