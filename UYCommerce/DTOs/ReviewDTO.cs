using System;
using System.ComponentModel.DataAnnotations;

namespace UYCommerce.DTOs
{
	public class ReviewDTO
	{
        [Required]
        public int ProductoId { get; set; }
		[Required]
		[MinLength(3,ErrorMessage = "Minimo 10 caracteres")]
        public string? Comentario { get; set; }
		[Required]
        [Range(1, 5, ErrorMessage = "La valoracion debe estar entre 1 y 5")]
        public int Valoracion { get; set; }
	}

    public class UpdateReviewDTO
    {
        [Required]
        public int ReviewId { get; set; }
        [Required]
        public int ProductoId { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Minimo 10 caracteres")]
        public string? Comentario { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "La valoracion debe estar entre 1 y 5")]
        public int Valoracion { get; set; }
    }
}

