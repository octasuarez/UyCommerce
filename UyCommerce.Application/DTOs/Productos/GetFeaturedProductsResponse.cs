using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.DTOs.Productos
{
    public class GetFeaturedProductsResponse
    {
        public List<string> ImagenesCarousel { get; set; } = [];
        public List<GetFeaturedProductsCategoriaDto> Categorias { get; set; } = [];
        public List<GetFeaturedProductsSkuDto> Skus { get; set; } = [];
    }

    public class GetFeaturedProductsCategoriaDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Imagen { get; set; } = string.Empty;
    }

    public class GetFeaturedProductsSkuDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public double Precio { get; set; }
        public List<string> Imagenes { get; set; } = [];
        public bool EsFavorito { get; set; } = false;
        public int ProductoPuntuacion { get; set; }
    }
}
