namespace UYCommerce.Models
{
    public class ProductoImagen
    {
        public int Id { get; set; }
        public virtual Producto? Producto { get; set; }
        public string? ImagenNombre { get; set; }
        public int Orden { get; set; }
    }
}