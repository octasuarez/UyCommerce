namespace UYCommerce.Models
{
    public class Respuesta
    {
        public int Id { get; set; }
        public int PreguntaId { get; set; }
        public virtual Pregunta? Pregunta { get; set; }
        public string? Contenido { get; set; }
        public DateTime Fecha { get; set; }
    }
}