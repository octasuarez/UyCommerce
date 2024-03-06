namespace UYCommerce.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public virtual Categoria? CategoriaPadre { get; set; }
        public virtual ICollection<Categoria>? SubCategorias { get; set; }
        public virtual ICollection<Producto>? Productos { get; set; }
        public bool MostrarEnInicio { get; set; } = false;
        public string? ImagenNombre { get; set; }
        public virtual ICollection<Atributo>? Atributos { get; set; }
    }
}