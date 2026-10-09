using System;
namespace UyCommerce.Domain.Entities
{
	public class AtributoValor
	{
		public int Id { get; set; }
		public Atributo? Atributo { get; set; }
		public string? Valor { get; set; }
		public virtual ICollection<Sku>? Skus { get; set; }
	}
}

