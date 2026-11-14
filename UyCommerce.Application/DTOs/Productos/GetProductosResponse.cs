using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.DTOs.Productos
{
    public class GetProductosResponse
    {
        public List<GetProductosSkuDto> Skus { get; set; } = [];
        public List<GetProductosCategoriaDto>? Categorias { get; set; }
        public GetProductosCategoriaSelectedDto? Categoria { get; set; }
        public List<GetProductosCategoriaDto> SubCategorias { get; set; } = [];
        public List<KeyValuePair<string, string>> Atributos { get; set; } = [];
        public List<GetProductosFiltrosDto> Filtros { get; set; } = [];
        public decimal NumberOfPages { get; set; }
        public int Page { get; set; }
        public int TotalItems { get; set; }
    }

    public class GetProductosSkuDto
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public List<string>? Imagenes { get; set; }
        public double Precio { get; set; }
        public double PrecioAnterior { get; set; }
        public int Puntuacion { get; set; }
        public bool EsFavorito { get; set; } = false;
    }

    public class GetProductosCategoriaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    public class GetProductosCategoriaSelectedDto : GetProductosCategoriaDto
    {
        public GetProductosCategoriaDto? CategoriaPadre { get; set; }
    }

    public class GetProductosFiltrosDto
    {
        public string Nombre { get; set; } = string.Empty;
        public List<string> Valores { get; set; } = [];
    }
}
