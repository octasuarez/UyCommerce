using System;
namespace UYCommerce.Models
{
	public class Review
	{
		public int Id { get; set; }
		public int ProductoId {get;set;}
		public int UsuarioId { get; set; }
		public virtual Usuario? Usuario { get; set; }
		public int Puntuacion { get; set; }
		public string? Texto { get; set; }
	}
}

