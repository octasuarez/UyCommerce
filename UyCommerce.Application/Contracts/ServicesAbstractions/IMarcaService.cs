using UyCommerce.Application.DTOs.Marcas;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IMarcaService
    {
        Task<GetMarcasResponse> GetMarcas();
        Task<GetMarcasWithProductosResponse> GetMarcasWithProductos();
        Task<Marca> GetMarcaById(int id);
        Task<Marca> GetMarcaAndProductosById(int id);
        Task DeleteMarca(int id);
        Task AddMarca(string nombre);
    }
}
