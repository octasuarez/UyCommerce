using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.DTOs
{
    public class GetSkuByKeyDto
    {
        public Producto Producto { get; set; } = new();
        public SkuDto Sku { get; set; } = new();
        public List<AtributoDto> Opciones { get; set; } = [];
        public ICollection<Pregunta> Preguntas { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];
        public ICollection<Sku> ProductosRelacionados { get; set; } = [];
        public Categoria Categoria { get; set; } = new();
        public ICollection<Sku> Favoritos { get; set; } = [];
        public double PuntuacionPromedio { get; set; }
    }
}
