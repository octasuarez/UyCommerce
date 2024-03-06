using System;
namespace UYCommerce.Models
{
	public class AtributoValor
	{
		public int Id { get; set; }
		public Atributo? Atributo { get; set; }
		public string? Valor { get; set; }
		public virtual ICollection<Sku>? Skus { get; set; }
	}
}

