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

        public UsuarioService(IUsuarioRepository usuarioRepository, IProductoRepository productoRepository)
        {
            _usuarioRepository = usuarioRepository;
            this._productoRepository = productoRepository;
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

        
    }
}
