using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.Persistance
{
    public interface IUsuarioRepository : IGenericRepository<Usuario>
    {
        Task<Usuario> GetUsuarioAndFavoritos(int id);
        Task<List<Usuario>> GetUsuariosClientesAsync();
    }
}
