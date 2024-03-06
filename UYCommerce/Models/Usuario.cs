namespace UYCommerce.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string? Nombre  { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }

        public Rol? Rol { get; set; }
        public Carrito Carrito { get; set; } = new();
        public virtual ICollection<Orden>? Ordenes { get; set; }
        public virtual ICollection<Producto>? Favoritos { get; set; }
        public virtual ICollection<Pregunta>? Preguntas { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }

    }
}