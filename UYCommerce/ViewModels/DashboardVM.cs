using System;
using UYCommerce.Models;

namespace UYCommerce.ViewModels
{
    public class DashboardVM
    {
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public required ICollection<Orden> Ordenes { get; set; }
        public required ICollection<ProductoOrden> Productos { get; set; }
        public ICollection<PuntoGrafica> PuntosVentas { get; set; }
        public ICollection<PuntoGrafica> PuntosGanacias { get; set; }
        public int CantidadVentas { get { return Productos.Count; } }
        public double Total { get { return Productos.Sum(p => p.Precio * p.Cantidad); } }
    }

    public class PuntoGrafica {

        public string x { get; set; }
        public double y { get; set; }
    }
}

