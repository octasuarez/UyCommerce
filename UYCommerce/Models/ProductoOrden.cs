using System;
using Microsoft.EntityFrameworkCore;

namespace UYCommerce.Models
{
    [PrimaryKey(nameof(OrdenId), nameof(SkuId))]
    public class ProductoOrden
	{
		public required string OrdenId { get; set; }
		public int SkuId { get; set; }
		public virtual Sku? Sku { get; set; }
		public string? Nombre { get; set; }
		public double Precio { get; set; }
		public int Cantidad { get; set; }
	}
}

