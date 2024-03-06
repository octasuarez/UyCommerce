using System;
namespace UYCommerce.Models
{
	public class Carrito
	{
		public int Id { get; set; }
		public ICollection<ProductoCarrito>? Productos { get; set; }
		public int UsuarioId { get; set; }
		public Usuario? Usuario { get; set; }


        public double? GetTotal()
        {

			double? total = Productos!.Sum(p => p.Sku!.Precio * p.Cantidad);

			return total;
		}
	}
}

