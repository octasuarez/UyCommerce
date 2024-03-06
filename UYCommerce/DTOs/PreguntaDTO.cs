using System;
using UYCommerce.Models;

namespace UYCommerce.DTOs
{
	public class PreguntaDTO
	{
        public int ProductoId { get; set; }
        public required string Contenido { get; set; }
        public string? NombreUsuario { get; set; }
        public DateTime Fecha { get; set; }
    }
}

