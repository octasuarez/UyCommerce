namespace UyCommerce.Application.DTOs.Preguntas
{
	public class CreatePreguntaRequest
	{
        public int ProductoId { get; set; }
        public required string Contenido { get; set; }
        public DateTime Fecha { get; set; }
    }
}

