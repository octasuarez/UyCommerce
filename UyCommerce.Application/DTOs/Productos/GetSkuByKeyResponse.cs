using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.DTOs.Productos
{
    public class GetSkuByKeyResponse
    {
        public Producto Producto { get; set; } = new();
        public SkuDto Sku { get; set; } = new();
        public List<AtributoDto> Opciones { get; set; } = [];
        public List<Pregunta> Preguntas { get; set; } = [];
        public List<Review> Reviews { get; set; } = [];
        public List<Sku> ProductosRelacionados { get; set; } = [];
        public Categoria Categoria { get; set; } = new();
        public List<Sku> Favoritos { get; set; } = [];
        public double PuntuacionPromedio { get; set; }
    }
}
