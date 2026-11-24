namespace UyCommerce.Application.DTOs.Marcas
{
    public class GetMarcasWithProductosResponse
    {
        public List<GetMarcasWithProductosMarcaDto> Marcas { get; set; } = [];
    }
    public class GetMarcasWithProductosMarcaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public List<GetMarcasWithProductosProductoDto> Productos { get; set; } = [];
    }

    public class GetMarcasWithProductosProductoDto
    {
        public int Id { get; set; }
    }
}
