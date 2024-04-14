using System;
using System.ComponentModel.DataAnnotations;
using UYCommerce.Models;

namespace UYCommerce.DTOs
{
	public class CategoriaDTO
	{
        [Required(ErrorMessage = "Campo obligatorio")]
        public string? Nombre { get; set; }
        [Required(ErrorMessage = "Campo obligatorio")]
        public bool MostrarEnInicio { get; set; } = false;
        [Required(ErrorMessage = "Campo obligatorio")]
        public bool EsCategoriaPadre { get; set; } = false;
        [Required(ErrorMessage = "Campo obligatorio")]
        public IFormFile? Imagen { get; set; }

        public int CategoriaPadreId { get; set; }

        public virtual ICollection<int>? Atributos { get; set; }
    }
}

