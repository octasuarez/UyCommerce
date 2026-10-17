using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.DTOs
{
    public class AddSkuAProductoDto
    {
            public int ProductoId { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public double Precio { get; set; }
            public int Stock { get; set; }
            public List<CreateImageDto> Imagenes { get; set; } = [];
            public List<int> AtributoValores { get; set; } = [];
            public List<Atributo> Atributos { get; set; } = [];
            public Producto Producto { get; set; } = new();
    }
}
