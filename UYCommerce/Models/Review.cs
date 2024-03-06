using System;
namespace UYCommerce.Models
{
	public class Review
	{
		public int Id { get; set; }
		public int SkuId {get;set;}
		public int UsuarioId { get; set; }
		public double Puntuacion { get; set; }
		public string? Texto { get; set; }


	}
}

