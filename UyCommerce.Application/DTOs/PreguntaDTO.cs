
namespace UyCommerce.Application.DTOs
{
	public class PreguntaDTO
	{
        public int ProductoId { get; set; }
        public required string Contenido { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Fecha { get; set; }
    }
}

