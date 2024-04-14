using System;
using System.ComponentModel.DataAnnotations;

namespace UYCommerce.DTOs
{
	public class ContactoDTO
	{
		[Required(ErrorMessage = "Escriba un asunto")]
		public string? Asunto { get; set; }
		[Required(ErrorMessage = "Escriba un nombre")]
        [MinLength(3, ErrorMessage = "Minimo 3 caracteres")]
        public string? Nombre { get; set; }
		[Required(ErrorMessage = "Escriba un mensaje")]
		[MinLength(10,ErrorMessage = "Minimo 5 caracteres")]
		[MaxLength(200,ErrorMessage = "Maximo 200 caracteres")]
		public string? Mensaje { get; set; }
	}
}

