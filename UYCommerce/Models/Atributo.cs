using System.ComponentModel.DataAnnotations;

namespace UYCommerce.Models
{
    public class Atributo
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public ICollection<AtributoValor>? Valores { get; set; }
        public virtual ICollection<Categoria>? Categorias { get; set; }
        public virtual ICollection<Producto>? Productos { get; set; }
    }
}