using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.DTOs.Atributos
{
    public class GetAtributosResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public List<GetAtributoValorDto> Valores { get; set; } = [];
        public virtual List<GetAtributosCategoriaDto> Categorias { get; set; } = [];
        public virtual List<GetAtributosProductoDto> Productos { get; set; } = [];
    }

    public class GetAtributoValorDto
    {
        public string Valor { get; set; } = string.Empty;
    }

    public class GetAtributosCategoriaDto
    {
        public string Nombre { get; set; } = string.Empty;
    }

    public class GetAtributosProductoDto
    {
        public string Nombre { get; set; } = string.Empty;
    }
}
