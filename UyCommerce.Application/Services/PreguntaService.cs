using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Services
{
    public class PreguntaService : IPreguntaService
    {
        private readonly IPreguntaRepository _preguntaRepository;

        public PreguntaService(IPreguntaRepository preguntaRepository)
        {
            _preguntaRepository = preguntaRepository;
        }
        public async Task<List<Pregunta>> GetPreguntas(bool answered)
        {
            return await _preguntaRepository.GetPreguntas(answered);
        }
    }
}
