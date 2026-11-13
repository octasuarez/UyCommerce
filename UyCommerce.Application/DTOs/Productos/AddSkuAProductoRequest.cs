using UyCommerce.Application.DTOs.Images;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.DTOs.Productos
{
    public class AddSkuAProductoRequest
    {
            public int ProductoId { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public double Precio { get; set; }
            public int Stock { get; set; }
            public List<CreateImageRequest> Imagenes { get; set; } = [];
            public List<int> AtributoValores { get; set; } = [];
    }
}
