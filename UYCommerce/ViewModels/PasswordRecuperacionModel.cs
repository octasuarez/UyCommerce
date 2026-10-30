using System;
using System.ComponentModel.DataAnnotations;

namespace UYCommerce.ViewModels
{
    public class PasswordRecuperacionModel
    {
        [Required]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "Crea una contraseña")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{6,15}$", ErrorMessage = "Al menos 6 caracteres\nuna minuscula\nuna mayuscula\nun numero")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Correo electronico invalido")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}

