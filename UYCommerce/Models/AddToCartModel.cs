using System;
namespace UYCommerce.Models
{
	public class AddToCartModel
	{
		public required string skuId { get; set; }
		public required string quantity { get; set; }
	}
}

