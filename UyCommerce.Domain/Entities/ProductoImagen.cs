namespace UyCommerce.Domain.Entities
{
    public class ProductoImagen
    {
        public int Id { get; set; }
        public virtual Producto? Producto { get; set; }
        public string? ImagenNombre { get; set; }
    }
}