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
    public class PreguntaService : IPreguntaService
    {
        private readonly IPreguntaRepository _preguntaRepository;

        public PreguntaService(IPreguntaRepository preguntaRepository)
        {
            _preguntaRepository = preguntaRepository;
        }

        public async Task<Pregunta> GetPreguntaByIdAsync(int id)
        {
            return await _preguntaRepository.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(Pregunta), id);
        }

        public async Task<Pregunta> GetPreguntaByRespuestaIdAsync(int respuestaId)
        {
            return await _preguntaRepository.GetPreguntaByRespuestaId(respuestaId)
                ?? throw new NotFoundException(nameof(Pregunta),respuestaId);
        }

        public async Task<List<Pregunta>> GetPreguntas(bool? answered)
        {
            return await _preguntaRepository.GetPreguntas(answered);
        }

        public async Task UpdatePreguntaRespuestaAsync(RespuestaDTO respuesta)
        {
            if(respuesta.PreguntaId < 0 || string.IsNullOrEmpty(respuesta.Contenido))
            {
                throw new Exception("Respuesta invalida.");
            }
            var pregunta = await GetPreguntaByIdAsync(respuesta.PreguntaId);
            pregunta.Respuesta = new Respuesta()
            {
                Contenido = respuesta.Contenido,
                Fecha = DateTime.Now,
                PreguntaId = pregunta.Id
            };
            await _preguntaRepository.UpdateAsync(pregunta);
        }
    }
}
