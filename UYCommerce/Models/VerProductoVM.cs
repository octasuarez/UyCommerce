using System;
namespace UYCommerce.Models
{
    public class VerProductoVM
    {
        public Producto Producto { get; set; } = new();
        public required Sku Sku { get; set; }
        public IEnumerable<IGrouping<Atributo,AtributoValor>>? Opciones { get; set; }
        public ICollection<Pregunta> Preguntas { get; set; } = new List<Pregunta>();
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<Producto>? ProductosRelacionados { get; set; }
        public Categoria? Categoria { get; set; }


        public double GetPuntuacionPromedio() {

            if(Reviews is null) { return 0; }
            return Reviews.Average(r => r.Puntuacion);
        }
    }

}

