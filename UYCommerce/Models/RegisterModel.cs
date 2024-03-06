using System;
using System.ComponentModel.DataAnnotations;

namespace UYCommerce.Models
{
	public class RegisterModel
	{

        [Required(ErrorMessage = "Escriba su Nombre")]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "Escriba su Email")]
        [EmailAddress]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Crea una contraseña")]
        [Compare("ConfirmPassword",ErrorMessage = "Las contraseñas no coinciden")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{6,15}$", ErrorMessage = "Al menos 6 caracteres\nuna minuscula\nuna mayuscula\nun numero")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Confirma tu contraseña")]
        [DataType(DataType.Password)]
        public required string ConfirmPassword { get; set; }

    }
}

