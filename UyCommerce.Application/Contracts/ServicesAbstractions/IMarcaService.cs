using UyCommerce.Application.DTOs.Marcas;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IMarcaService
    {
        Task<GetMarcasResponse> GetMarcas();
        Task<GetMarcasWithProductosResponse> GetMarcasWithProductos();
        Task DeleteMarca(int id);
        Task AddMarca(string nombre);
    }
}
