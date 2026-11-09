using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IMarcaService
    {
        Task<List<Marca>> GetMarcas();
        Task<List<Marca>> GetMarcasWithProductos();
        Task<Marca> GetMarcaById(int id);
        Task<Marca> GetMarcaAndProductosById(int id);
        Task DeleteMarca(int id);
        Task AddMarca(string nombre);
    }
}
