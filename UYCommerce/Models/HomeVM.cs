﻿using System;
namespace UYCommerce.Models
{
	public class HomeVM
	{
		public ICollection<string>? ImagenesCarousel { get; set; }
        public ICollection<Categoria>? Categorias { get; set; }
        public ICollection<Sku>? Skus { get; set; }
        public ICollection<Sku>? Favoritos { get; set; }
        public ICollection<CarouselImage>? Carousel { get; set; }
    }
}

