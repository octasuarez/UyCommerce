using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.DTOs;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IAtributoService
    {
        Task<List<Atributo>> GetAtributos();
        Task AddAtributoValor(AgregarValorDTO atributoDto);
        Task AddAtributo(string nombre);
    }
}
