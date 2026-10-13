using System.ComponentModel.DataAnnotations;

namespace UYCommerce.ViewModels
{
    public class CreateCategoriaModel
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
