using System;
using UyCommerce.Application.DTOs;
using UyCommerce.Domain.Entities;

namespace UYCommerce.ViewModels
{
    public class VerProductoVM
    {
        public Producto Producto { get; set; } = new();
        public SkuDto Sku { get; set; } = new();
        public IEnumerable<IGrouping<Atributo,AtributoValor>>? Opciones { get; set; }
        public ICollection<Pregunta> Preguntas { get; set; } = new List<Pregunta>();
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<Sku>? ProductosRelacionados { get; set; }
        public Categoria? Categoria { get; set; }
        public ICollection<Sku>? Favoritos { get; set; }
        public double PuntuacionPromedio { get; set; }

    }

}

