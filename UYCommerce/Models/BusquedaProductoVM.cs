using System;
namespace UYCommerce.Models
{
	public class BusquedaProductoVM
	{
		public ICollection<Categoria>? Categorias { get; set; }
        public ICollection<Producto>? Productos { get; set; }
        public ICollection<Sku>? Skus { get; set; }
        public Categoria? Categoria { get; set; }
		public ICollection<Sku>? Favoritos { get; set; }
		public List<KeyValuePair<string, string>>? Atributos { get; set; }
		public decimal NumberOfPages { get; set; }
		public int Pag { get; set; }
    }
}

