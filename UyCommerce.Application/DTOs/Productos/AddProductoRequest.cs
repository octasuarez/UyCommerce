using UyCommerce.Application.DTOs.Images;

namespace UyCommerce.Application.DTOs.Productos
{
    public class AddProductoRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public int MarcaId { get; set; }
        public List<CreateImageRequest> Imagenes { get; set; } = new List<CreateImageRequest>();
        public List<int> Atributos { get; set; } = new List<int>();
    }
}
