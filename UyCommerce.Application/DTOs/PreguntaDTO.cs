using System;
using System.Collections.Generic;
using System.Text;

namespace UyCommerce.Application.DTOs
{
    public class PreguntaDto
    {
        public string Contenido { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string Fecha { get; set; } = string.Empty;
    }
}
