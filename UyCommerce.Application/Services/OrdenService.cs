using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Domain.Entities;
using UyCommerce.Domain.Exceptions;

namespace UyCommerce.Application.Services
{
    public class OrdenService : IOrdenService
    {
        private readonly IOrdenRepository _ordenRepository;

        public OrdenService(IOrdenRepository ordenRepository)
        {
            _ordenRepository = ordenRepository;
        }
        public async Task<List<Orden>> GetOrdenesByDateAsync(DateTime? startDate, DateTime? endDate)
        {

            startDate = startDate ?? DateTime.Now.AddMonths(-4).Date;
            endDate = endDate?? DateTime.Now.Date;

            return await _ordenRepository.GetOrdenesByDateAsync((DateTime)startDate, (DateTime)endDate);
        }

        public async Task<List<Orden>> GetOrdenesAsync()
        {
            return await _ordenRepository.GetOrdenesAsync();
        }

        public Task DeleteOrdenAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Orden>> GetOrdenesUsuariosAsync()
        {
            return await _ordenRepository.GetOrdenesUsuariosAsync();
        }

        public async Task<List<Orden>> GetOrdenesByProductoIdAsync(int productoId)
        {
            return await _ordenRepository.GetOrdenesByProductoIdAsync(productoId);
        }

        public async Task<List<Orden>> GetOrdenesByUsuarioId(int userId)
        {
            return await _ordenRepository.GetOrdenesByUsuarioId(userId);
        }

        public async Task<Orden> GetOrdenById(int id)
        {
            var orden = await _ordenRepository.GetOrdenCompleteById(id)
                ?? throw new NotFoundException(nameof(Orden), id);
            return orden;
        }
    }
}
