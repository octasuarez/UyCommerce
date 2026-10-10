using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace UyCommerce.Application.DTOs
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

