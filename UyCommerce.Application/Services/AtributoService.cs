using Mapster;
using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Application.DTOs;
using UyCommerce.Domain.Entities;
using UyCommerce.Domain.Exceptions;

namespace UyCommerce.Application.Services
{
    public class AtributoService : IAtributoService
    {
        private readonly IAtributoRepository _atributoRepository;
        public AtributoService(IAtributoRepository atributoRepository) => _atributoRepository = atributoRepository;

        public async Task AddAtributoAsync(string nombre)
        {
            var atributoDb = await _atributoRepository.GetAtributoByNameAsync(nombre);
            if (atributoDb is not null) throw new ArgumentOutOfRangeException($"{nameof(Atributo)} con nombre {nombre} ya existe.");
            Atributo newAtributo = new() { Nombre =  nombre };
            await _atributoRepository.CreateAsync(newAtributo);
        }

        public async Task AddAtributoValorAsync(AgregarValorDTO atributoDto)
        {
            var atributoDb = await _atributoRepository.GetAtributoConValoresByIdAsync(atributoDto.atributoId)
                ?? throw new NotFoundException(nameof(Atributo),atributoDto.atributoId);

            var valorDb = atributoDb?.Valores?.First(v => v.Valor.Equals(atributoDto.Valor, StringComparison.OrdinalIgnoreCase));
            if (valorDb is not null) 
                throw new Exception($"{nameof(AtributoValor)} already exists");

            AtributoValor newAtributoValor = new() { Atributo = atributoDb, Valor =  atributoDto.Valor };
            atributoDb.Valores ??= [];
            atributoDb.Valores.Add(newAtributoValor);
            await _atributoRepository.UpdateAsync(atributoDb);
        }

        public async Task<List<Atributo>> GetAtributosAsync()
        {
            return await _atributoRepository.GetAtributosAsync();
        }
    }
}
