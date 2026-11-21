using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.DTOs.Reviews
{
    public class GetReviewsResponse
    {
        public List<GetReviewDto> Reviews { get; set; } = [];
    }

    public class GetReviewDto
    {
        public int Id { get; set; }
        public GetReviewsProductoDto? Producto { get; set; } = new();
        public GetReviewsUsuarioDto Usuario { get; set; } = new();
        public int Puntuacion { get; set; }
        public string Texto { get; set; } = string.Empty;
    }

    public class GetReviewsProductoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public List<GetReviewsProductoImagenDto> Imagenes { get; set; } = [];
    }

    public class GetReviewsProductoImagenDto
    {
        public string ImagenNombre { get; set; } = string.Empty;
    }

    public class GetReviewsUsuarioDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
