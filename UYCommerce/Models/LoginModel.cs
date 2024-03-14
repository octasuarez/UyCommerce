using System;
using System.ComponentModel.DataAnnotations;

namespace UYCommerce.Models
{
	public class LoginModel
	{
		[Required(ErrorMessage = "Escriba su email")]
		[MinLength(5)]
		[EmailAddress(ErrorMessage ="Solo direcciones de correo electronico")]
		public required string Email { get; set; }

		[Required(ErrorMessage = "Escriba su constraseña")]
		[DataType(DataType.Password)]
		public required string Password { get; set; }

		public bool	RememberMe { get; set; }
	}
}

