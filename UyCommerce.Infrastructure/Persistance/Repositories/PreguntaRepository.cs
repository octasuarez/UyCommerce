using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Infrastructure.Persistance.Repositories
{
    public class PreguntaRepository : GenericRepository<Pregunta>, IPreguntaRepository
    {
        public PreguntaRepository(ShopContext context) : base(context)
        {

        }

        public async Task<Pregunta?> GetPreguntaByRespuestaId(int respuestaId)
        {
            return await _context.Preguntas
                .Where(p => p.Respuesta != null && p.Respuesta.Id == respuestaId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Pregunta>> GetPreguntas(bool? answered)
        {
            var preguntas = _context.Preguntas
                .AsNoTracking()
                .Include(p => p.Usuario)
                .Include(p => p.Producto).ThenInclude(pr => pr.Imagenes)
                .Include(p => p.Respuesta)
                .AsQueryable();
            if (answered is not null)
            {
                preguntas = preguntas.Where(p => answered == true ? p.Respuesta != null : p.Respuesta == null);
            }
            return await preguntas.ToListAsync();
        }
    }
}
