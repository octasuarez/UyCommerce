using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.Persistance
{
    public interface IPreguntaRepository : IGenericRepository<Pregunta>
    {
        Task<List<Pregunta>> GetPreguntas(bool? answered);
        Task<Pregunta?> GetPreguntaByRespuestaId(int respuestaId);
    }
}
