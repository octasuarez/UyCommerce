using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.DTOs
{
    public class SkuFavoritoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public int Stock { get; set; }
        public double Precio { get; set; }
        public double PrecioAnterior { get; set; }
        public ICollection<AtributoValor> AtributosValores { get; set; } = [];
        public List<string> Imagenes { get; set; } = [];
        public bool IsInCart { get; set; } = false;
    }
}
