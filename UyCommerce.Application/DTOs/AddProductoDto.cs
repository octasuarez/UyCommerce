

namespace UyCommerce.Application.DTOs
{
    public class AddProductoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public int MarcaId { get; set; }
        public List<CreateImageDto> Imagenes { get; set; } = new List<CreateImageDto>();
        public List<int> Atributos { get; set; } = new List<int>();
    }
}
