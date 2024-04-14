using System;
using System.ComponentModel.DataAnnotations;

namespace UYCommerce.Models
{
	public class ProductoModel
	{
        [Required(ErrorMessage = "Agrega un Nombre")]
        [MinLength(3,ErrorMessage ="Minimo 3 caracteres")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Agrega una Descripcion")]
        [MinLength(3,ErrorMessage = "Minimo 50 caracteres")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "Selecciona una Categoria")]
        [Range(0, int.MaxValue, ErrorMessage = "Selecciona una Categoria")]
        public int CategoriaId { get; set; }

        public int MarcaId { get; set; }
        [Required(ErrorMessage = "Sube al menos 1 imagen")]
        public IFormFileCollection Imagenes { get; set; }

        public List<int>? Atributos { get; set; }
	}
}

