using System;
using System.ComponentModel.DataAnnotations;

namespace UYCommerce.Models
{
	public class Orden
	{
		[Key]
		public required string Id { get; set; }
		public int? UsuarioId { get; set; }
		public string? NombreComprador { get; set; }
		public OrdenDireccion? Direccion { get; set; }
		public DateTime FechaDeCompra { get; set; }
		public DateTime? FechaDeEntrega { get; set; }
        public string? Estado { get; set; }
		public ICollection<ProductoOrden>? Productos { get; set; }
		public string? MetodoDePago { get; set; }
		public double? Total { get; set; }
	}

    public class OrdenDireccion
    {

        public int Id { get; set; }
        public string? address_line_1 { get; set; }
        public string? address_line_2 { get; set; }
        public string? admin_area_2 { get; set; }
        public string? admin_area_1 { get; set; }
        public string? postal_code { get; set; }
        public string? country_code { get; set; }
    }
}

