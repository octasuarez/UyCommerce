using System;
namespace UyCommerce.Domain.Entities
{
	public class AddToCartModel
	{
		public required string skuId { get; set; }
		public required string quantity { get; set; }
	}
}

