using System;
using System.Collections.Generic;
using System.Text;

namespace UyCommerce.Application.DTOs.Ordenes
{
    public class GetOrdenesByUsuarioIdResponse
    {
        public int Id { get; set; }
        public DateTime FechaDeCompra { get; set; }
        public DateTime? FechaDeEntrega { get; set; }
        public double Total { get; set; }
        public GetOrdenesByUsuarioIdEstadoDto Estado { get; set; } = new();
    }

    public class GetOrdenesByUsuarioIdEstadoDto
    {
        public string Descripcion { get; set; } = string.Empty;
    }
}
