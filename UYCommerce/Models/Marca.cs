namespace UYCommerce.Models
{
    public class Marca
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public virtual ICollection<Producto>? Productos { get; set; }
    }
}