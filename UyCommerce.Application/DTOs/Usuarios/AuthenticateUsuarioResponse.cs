namespace UyCommerce.Application.DTOs.Usuarios
{
    public class AuthenticateUsuarioResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Activo { get; set; } = false;
        public AuthenticateUsuarioCarritoDto Carrito { get; set; } = new();
        public AuthenticateUsuarioRol Rol { get; set; }
    }

    public enum AuthenticateUsuarioRol
    {
        User = 1,
        Admin = 2
    }

    public class AuthenticateUsuarioCarritoDto{
        public int Id { get; set; }
        public List<AuthenticateUsuarioProductoCarritoDto> Productos { get; set; } = [];
    }

    public class AuthenticateUsuarioProductoCarritoDto
    {
        public int SkuId { get; set; }
    }
}
