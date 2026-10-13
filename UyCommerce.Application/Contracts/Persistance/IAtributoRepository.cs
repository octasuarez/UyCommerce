using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.Persistance
{
    public interface IAtributoRepository : IGenericRepository<Atributo>
    {
        Task<List<Atributo>> GetAtributosAsync();
        Task<List<Atributo>?> GetAtributosByCategoriaAsync(Categoria categoria); 
    }
}
