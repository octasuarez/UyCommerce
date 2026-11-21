namespace UyCommerce.Application.DTOs.Ordenes
{
    public class GetOrdenByIdResponse
    {
        public int Id { get; set; }
        public GetOrdenByIdDireccionDto Direccion { get; set; } = new();
        public DateTime FechaDeCompra { get; set; }
        public DateTime? FechaDeEntrega { get; set; }
        public GetOrdenByIdEstadoDto Estado { get; set; } = new();
        public List<GetOrdenByIdProductoOrdenDto>? Productos { get; set; }
        public string? MetodoDePago { get; set; }
        public double? Total { get; set; }
    }

    public class GetOrdenByIdDireccionDto
    {
        public string? address_line_1 { get; set; }
        public string? address_line_2 { get; set; }
        public string? admin_area_2 { get; set; }
        public string? admin_area_1 { get; set; }
        public string? postal_code { get; set; }
        public string? country_code { get; set; }
    }

    public class GetOrdenByIdEstadoDto
    {
        public string Descripcion { get; set; } = string.Empty;
    }

    public class GetOrdenByIdProductoOrdenDto
    {
        public required GetOrdenByIdProductoOrdenSkuDto Sku { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public double Precio { get; set; }
        public int Cantidad { get; set; }
    }

    public class GetOrdenByIdProductoOrdenSkuDto
    {
        public string Key { get; set; } = string.Empty;
        public List<GetOrdenByIdAtributoValorDto>? AtributosValores { get; set; }
        public List<GetOrdenByIdSkuImagen>? Imagenes { get; set; }
    }

    public class GetOrdenByIdAtributoValorDto
    {
        public GetOrdenByIdAtributoDto Atributo { get; set; } = new();
        public string Valor { get; set; } = string.Empty;
    }
    public class GetOrdenByIdAtributoDto
    {
        public string Nombre { get; set; } = string.Empty;
    }

    public class GetOrdenByIdSkuImagen
    {
        public string ImagenNombre { get; set; } = string.Empty;
    }
}
