using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
public interface ICategoriaService
    {
        Task<List<Categoria>> GetCategoriasAsync();
    }
}
