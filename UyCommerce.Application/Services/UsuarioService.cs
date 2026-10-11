using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Domain.Entities;
using UyCommerce.Domain.Exceptions;

namespace UyCommerce.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<List<Usuario>> GetUsuariosClientesAsync()
        {
            return await _usuarioRepository.GetUsuariosClientesAsync();
        }

        public async Task<Usuario> GetUsuarioFavoritosByIdAsync(int id)
        {
            var usuario = await _usuarioRepository.GetUsuarioAndFavoritos(id);
            if(usuario is null)
            {
                throw new NotFoundException(nameof(Usuario), id);
            }

            return usuario;
        }
    }
}
