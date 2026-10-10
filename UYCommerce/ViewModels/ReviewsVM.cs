using System;
using UyCommerce.Domain.Entities;

namespace UYCommerce.ViewModels
{
	public class ReviewsVM
	{
		public List<Review>? Reviews { get; set; }
		public List<Producto>? Productos { get; set; }
	}
}

