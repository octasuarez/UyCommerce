using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.Persistance
{
    public interface IUsuarioRepository : IGenericRepository<Usuario>
    {
        Task<Usuario?> GetUsuarioAndFavoritos(int id);
        Task<Usuario?> GetUsuarioByEmailAsync(string email);
        Task<List<Usuario>> GetUsuariosClientesAsync();
        Task<List<Rol>> GetRolesUsuarios();
        Task<Rol?> GetRolByNombre(string nombre);
        Task<Codigo?> GetCodigoRecuperacionUsuarioAsync(int id);
        Task<int> CreateCodigoRecuperacion(Codigo codigo);
        Task<bool> UpdateCodigoRecuperacion(Codigo codigo);
    }
}
