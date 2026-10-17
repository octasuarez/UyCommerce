using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.DTOs
{
    public class SkuDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public virtual Producto Producto { get; set; } = new();
        public string Nombre { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public int Stock { get; set; }
        public double Precio { get; set; }
        public double PrecioAnterior { get; set; }
        public ICollection<AtributoValor> AtributosValores { get; set; } = [];
        public ICollection<SkuImagen> Imagenes { get; set; } = [];
    }
}
