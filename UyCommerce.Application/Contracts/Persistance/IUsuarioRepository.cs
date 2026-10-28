using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.Persistance
{
    public interface IUsuarioRepository : IGenericRepository<Usuario>
    {
        Task<Usuario?> GetUsuarioAndFavoritos(int id);
        Task<Usuario?> GetUsuarioByEmailAsync(string email);
        Task<List<Usuario>> GetUsuariosClientesAsync();
    }
}
