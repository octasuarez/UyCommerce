using Mapster;
using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Services
{
    public class AtributoService : IAtributoService
    {
        private readonly IAtributoRepository _atributoRepository;

        public AtributoService(IAtributoRepository atributoRepository)
        {
            _atributoRepository = atributoRepository;
        }

        public IAtributoRepository AtributoRepository { get; }

        public async Task<List<Atributo>> GetAtributosAsync()
        {
            return await _atributoRepository.GetAtributosAsync();
        }
    }
}
