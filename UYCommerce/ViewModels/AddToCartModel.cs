using System;
namespace UYCommerce.ViewModels
{
	public class AddToCartModel
	{
		public required string skuId { get; set; }
		public required string quantity { get; set; }
	}
}

