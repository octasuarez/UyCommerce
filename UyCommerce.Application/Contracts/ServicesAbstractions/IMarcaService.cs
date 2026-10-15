using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IMarcaService
    {
        Task<List<Marca>> GetMarcasAsync();
        Task<List<Marca>> GetMarcasWithProductosAsync();
        Task<Marca> GetMarcaByIdAsync(int id);
        Task<Marca> GetMarcaAndProductosByIdAsync(int id);
        Task DeleteMarcaAsync(int id);
        Task AddMarcaAsync(string nombre);
    }
}
