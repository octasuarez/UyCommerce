using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.Persistance
{
    public interface IMarcaRepository : IGenericRepository<Marca>
    {
        Task<Marca> GetMarcaAndProductosAsync(int id);
        Task<Marca?> GetMarcaByNombre(string nombre);
        Task<List<Marca>> GetMarcasWithProductosAsync();
    }
}
