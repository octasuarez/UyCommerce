using System;
using System.ComponentModel.DataAnnotations;

namespace UYCommerce.Models
{
	public class AgregarSkuModel
	{
		[Required]
		public int ProductoId { get; set; }

		[Required(ErrorMessage = "Es necesario un nombre")]
		public string Nombre { get; set; }
		[Required(ErrorMessage = "Es necesario poner un precio")]
        [Range(0, int.MaxValue, ErrorMessage = "El precio debe ser mayor")]
		public double Precio { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor")]
		[Required(ErrorMessage = "Es necesario tener stock")]
		public int Stock { get; set; }

        public ICollection<IFormFile>? Imagenes { get; set; }

		public ICollection<int>? AtributoValores { get; set; }

		public ICollection<Atributo>? Atributos { get; set; }

		public Producto? Producto { get; set; }
	}
}

