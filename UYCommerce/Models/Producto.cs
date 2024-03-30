using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UYCommerce.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? NombreClave { get; set; }
        public string? Descripcion { get; set; }
        public int VecesComprado { get; set; } = 0;
        public virtual Categoria? Categoria { get; set; }
        public virtual Marca? Marca { get; set; }
        public virtual ICollection<Pregunta>? Preguntas { get; set; }
        public ICollection<ProductoImagen>? Imagenes { get; set; }
        public ICollection<Sku>? Skus { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<Atributo>? Atributos { get; set; }


        public double GetPuntuacionPromedio()
        {

            if (Reviews is null || !Reviews.Any()) { return 0; }
            return Reviews.Average(r => r.Puntuacion);
        }
    }
}

