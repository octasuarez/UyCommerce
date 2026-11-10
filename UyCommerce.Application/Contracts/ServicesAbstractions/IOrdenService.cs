using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.DTOs.Ordenes;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IOrdenService
    {
        Task<GetDashboardResponse> GetDashboard(DateTime? startDate, DateTime? endDate);
        Task<List<Orden>> GetOrdenes();
        Task DeleteOrden();
        Task<List<Orden>> GetOrdenesUsuarios();
        Task<List<Orden>> GetOrdenesByProductoId(int productoId);
        Task<List<Orden>> GetOrdenesByUsuarioId(int userId);
        Task<Orden> GetOrdenById(int id);
    }
}
