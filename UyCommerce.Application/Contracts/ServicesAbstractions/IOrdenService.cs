using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IOrdenService
    {
        Task<List<Orden>> GetOrdenesByDateAsync(DateTime? startDate, DateTime? endDate);
        Task<List<Orden>> GetOrdenesAsync();
        Task DeleteOrdenAsync();
    }
}
