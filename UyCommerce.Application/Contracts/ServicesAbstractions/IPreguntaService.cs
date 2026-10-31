using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.DTOs;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IPreguntaService
    {
        Task<List<Pregunta>> GetPreguntas(bool? answered);
        Task<Pregunta> GetPreguntaByIdAsync(int id);
        Task<Pregunta> GetPreguntaByRespuestaIdAsync(int respuestaId);
        Task UpdatePreguntaRespuestaAsync(RespuestaDTO respuesta);
    }
}
