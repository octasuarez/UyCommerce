using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.DTOs.Ordenes
{
    public class GetDashboardResponse
    {
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public List<Orden> Ordenes { get; set; } = [];
        public List<ProductoOrden> Productos { get; set; } = [];
        public List<PuntoGrafica> PuntosVentas { get; set; } = [];
        public List<PuntoGrafica> PuntosGanacias { get; set; } = [];
        public int CantidadVentas { get { return Productos.Count; } }
        public double Total { get { return Productos.Sum(p => p.Precio * p.Cantidad); } }
    }

    public record PuntoGrafica{public string x; public double y; };
}
