using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.DTOs;
using UyCommerce.Application.DTOs.Preguntas;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IPreguntaService
    {
        Task<List<Pregunta>> GetPreguntas(bool? answered);
        Task<Pregunta> GetPreguntaById(int id);
        Task<Pregunta> GetPreguntaByRespuestaId(int respuestaId);
        Task UpdatePreguntaRespuesta(RespuestaDTO respuesta);
        Task CreatePregunta(CreatePreguntaRequest preguntaDto, int userId);
    }
}
