using System;
namespace UyCommerce.Domain.Entities
{
	public class SkuImagen
	{
        public int Id { get; set; }
        public virtual Sku? Sku { get; set; }
        public string? ImagenNombre { get; set; }
    }
}

