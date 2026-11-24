namespace UyCommerce.Application.DTOs.Marcas
{
    public class GetMarcasResponse
    {
        public List<GetMarcasDto> Marcas { get; set; } = [];
    }

    public class GetMarcasDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
