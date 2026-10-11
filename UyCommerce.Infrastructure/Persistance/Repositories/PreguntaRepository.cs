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
        public PreguntaRepository(ShopContext context):base(context)
        {
            
        }

        public async Task<List<Pregunta>> GetPreguntas(bool answered)
        {
            return await _context.Preguntas
                .AsNoTracking()
                .Include(p => p.Usuario)
                .Include(p => p.Producto).ThenInclude(pr => pr.Imagenes)
                .Include(p => p.Respuesta)
                .Where(p => answered == true ? p.Respuesta != null : p.Respuesta == null)
                .ToListAsync();
        }
    }
}
