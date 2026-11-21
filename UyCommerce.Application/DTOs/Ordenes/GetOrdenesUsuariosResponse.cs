namespace UyCommerce.Application.DTOs.Ordenes
{
    public class GetOrdenesUsuariosResponse
    {
        public required GetOrdenesUsuariosOrdenDto Orden { get; set; }
        public required GetOrdenesUsuariosUsuarioDto Usuario { get; set; }
    }
    public class GetOrdenesUsuariosOrdenDto
    {
        public int Id { get; set; }
        public GetOrdenesUsuariosDireccionDto Direccion { get; set; } = new();
        public DateTime FechaDeCompra { get; set; }
        public DateTime? FechaDeEntrega { get; set; }
        public required GetOrdenesUsuariosEstadoDto Estado { get; set; }
        public List<GetOrdenesUsuariosProductoOrdenDto> Productos { get; set; } = [];
        public double Total { get; set; }
    }

    public class GetOrdenesUsuariosDireccionDto
    {
        public int Id { get; set; }
        public string address_line_1 { get; set; } = string.Empty;
        public string address_line_2 { get; set; } = string.Empty;
        public string admin_area_2 { get; set; } = string.Empty;
        public string admin_area_1 { get; set; } = string.Empty;
        public string postal_code { get; set; } = string.Empty;
        public string country_code { get; set; } = string.Empty;
    }

    public class GetOrdenesUsuariosEstadoDto
    {
        public string Descripcion { get; set; } = string.Empty;
    }

    public class GetOrdenesUsuariosProductoOrdenDto
    {
        public required GetOrdenesUsuariosProductoOrdenSkuDto Sku { get; set; }
        public double Precio { get; set; }
        public int Cantidad { get; set; }
    }

    public class GetOrdenesUsuariosProductoOrdenSkuDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
    }

    public class GetOrdenesUsuariosUsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}