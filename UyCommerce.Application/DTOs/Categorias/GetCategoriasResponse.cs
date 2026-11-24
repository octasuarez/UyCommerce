using System;
using System.Collections.Generic;
using System.Text;

namespace UyCommerce.Application.DTOs.Categorias
{
    public class GetCategoriasResponse
    {
        public List<GetCategoriasDto> Categorias { get; set; } = [];
    }

    public class GetCategoriasDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
