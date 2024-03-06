using System;
namespace UYCommerce.Models
{
    public class VerProductoVM
    {
        public Producto Producto { get; set; } = new();
        public ICollection<Atributo>? Atributos { get; set; }
        public ICollection<Pregunta> Preguntas { get; set; } = new List<Pregunta>();
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<Producto>? ProductosRelacionados { get; set; }
        public ICollection<string>? Imagenes { get; set; }
        public double Precio { get; set; }
        public Sku? Sku { get; set; }
        public Dictionary<string, string>? OpcionesElegidas { get; set; }
        public List<AtributoOpcion> Opciones { get; set; } = new();
        public double PrecioMin { get; set; }
        public double PrecioMax { get; set; }
    }

    public class AtributoOpcion
    {

        public string? AtributoNombre { get; set; }
        public Dictionary<string, string> Valores { get; set; } = new();
    }
}

