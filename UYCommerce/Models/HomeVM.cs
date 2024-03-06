using System;
namespace UYCommerce.Models
{
	public class HomeVM
	{
		public ICollection<string>? ImagenesCarousel { get; set; }
        public ICollection<Categoria>? Categorias { get; set; }
        public ICollection<Producto>? Productos { get; set; }
        public required ICollection<Producto> Favoritos { get; set; }
    }
}

