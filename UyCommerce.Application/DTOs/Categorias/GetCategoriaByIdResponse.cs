using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.DTOs.Categorias
{
    public class GetCategoriaByIdResponse
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public virtual ICollection<Producto>? Productos { get; set; }
        public bool MostrarEnInicio { get; set; } = false;
        public string? ImagenNombre { get; set; }
        public virtual ICollection<Atributo>? Atributos { get; set; }
    }
}
