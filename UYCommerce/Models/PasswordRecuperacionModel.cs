using System;
using System.ComponentModel.DataAnnotations;

namespace UYCommerce.Models
{
    public class PasswordRecuperacionModel
    {
        [Required]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "Crea una contraseña")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{6,15}$", ErrorMessage = "Al menos 6 caracteres\nuna minuscula\nuna mayuscula\nun numero")]
        [Compare("ConfirmPassword", ErrorMessage = "Las contraseñas no coinciden")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirma tu contraseña")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}

