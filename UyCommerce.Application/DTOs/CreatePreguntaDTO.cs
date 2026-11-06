
namespace UyCommerce.Application.DTOs
{
	public class CreatePreguntaDTO
	{
        public int ProductoId { get; set; }
        public required string Contenido { get; set; }
        public DateTime Fecha { get; set; }
    }
}

