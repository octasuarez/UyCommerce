using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Application.DTOs.Ordenes;
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
        public async Task<GetDashboardResponse> GetDashboard(DateTime? startDate, DateTime? endDate)
        {
            GetDashboardResponse response = new();
            List<PuntoGrafica> puntos = [];
            List<PuntoGrafica> puntosGanacias = [];
            List<ProductoOrden> productos = [];

            startDate = startDate ?? DateTime.Now.AddMonths(-4).Date;
            endDate = endDate ?? DateTime.Now.Date;

            var ordenes = await _ordenRepository.GetOrdenesByDateAsync((DateTime)startDate, (DateTime)endDate);

            if (ordenes != null && ordenes.Count > 0)
            {
                productos = [.. ordenes.Where(o => o.Productos != null).SelectMany(o => o.Productos)];

                int i = 0;
                DateTime fecha = startDate.Value;
                while (fecha < endDate.Value)
                {
                    fecha = startDate.Value.AddMonths(i);

                    puntos.Add(new PuntoGrafica
                    {
                        x = DateTimeFormatInfo.CurrentInfo.GetMonthName(fecha.Month) + " " + fecha.Year,
                        y = ordenes.Where(o => o.FechaDeCompra.Month == fecha.Month
                        && o.FechaDeCompra.Year == fecha.Year)
                        .Where(o => o.Productos != null).SelectMany(o => o.Productos).Count()
                    });

                    puntosGanacias.Add(new PuntoGrafica
                    {
                        x = DateTimeFormatInfo.CurrentInfo.GetMonthName(fecha.Month) + " " + fecha.Year,
                        y = (double)ordenes.Where(o => o.FechaDeCompra.Month == fecha.Month
                        && o.FechaDeCompra.Year == fecha.Year).Sum(o => o.Total)!
                    });

                    i += 1;
                }
            }

            response.FechaDesde = (DateTime)startDate;
            response.FechaHasta = (DateTime)endDate;
            response.Productos = productos;
            response.PuntosVentas = puntos;
            response.PuntosGanacias = puntosGanacias;
            response.Ordenes = ordenes ?? [];

            return response;
        }

        public async Task<List<Orden>> GetOrdenes()
        {
            return await _ordenRepository.GetOrdenesAsync();
        }

        public Task DeleteOrden()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Orden>> GetOrdenesUsuarios()
        {
            return await _ordenRepository.GetOrdenesUsuariosAsync();
        }

        public async Task<List<Orden>> GetOrdenesByProductoId(int productoId)
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
