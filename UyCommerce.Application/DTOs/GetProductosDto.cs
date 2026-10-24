using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.DTOs
{
    public class GetProductosDto
    {
        public ICollection<Sku>? Skus { get; set; }
        public ICollection<Categoria>? Categorias { get; set; }
        public ICollection<Sku>? Favoritos { get; set; }
        public Categoria? Categoria { get; set; }
        public ICollection<Categoria>? SubCategorias { get; set; }
        public ICollection<Producto>? Productos { get; set; }
        public List<KeyValuePair<string, string>>? Atributos { get; set; }
        public IEnumerable<IGrouping<Atributo, AtributoValor>>? Filtros { get; set; }
        public decimal NumberOfPages { get; set; }
        public int Page { get; set; }
        public int TotalItems { get; set; }
    }
}
