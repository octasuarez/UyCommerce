using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.DTOs;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
public interface ICategoriaService
    {
        Task<List<Categoria>> GetCategoriasAsync();
        Task<Categoria?> GetCategoriaByIdAsync(int id);
        Task AddCategoriaAsync(CategoriaDTO categoriaDto, Stream? imgStream);
        Task FeatureCategoria(CategoriaDTO categoriaDto);
    }
}
