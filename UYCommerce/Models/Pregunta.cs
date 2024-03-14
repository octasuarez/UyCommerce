namespace UYCommerce.Models
{
    public class Pregunta
    {
        public int Id { get; set; }
        public virtual Producto? Producto { get; set; }
        public required virtual Usuario Usuario { get; set; }
        public required string Contenido { get; set; }
        public DateTime Fecha { get; set; }
        public virtual Respuesta? Respuesta { get; set; }

    }
}