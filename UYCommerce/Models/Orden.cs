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
		public string? Direccion { get; set; }
		public DateTime FechaDeCompra { get; set; }
		public DateTime? FechaDeEntrega { get; set; }
        public string? Estado { get; set; }
		public ICollection<ProductoOrden>? Productos { get; set; }
	}
}

