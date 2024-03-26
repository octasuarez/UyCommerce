using System;
namespace UYCommerce.Models
{
	public class SkuImagen
	{
        public int Id { get; set; }
        public virtual Sku? Sku { get; set; }
        public string? ImagenNombre { get; set; }
    }
}

