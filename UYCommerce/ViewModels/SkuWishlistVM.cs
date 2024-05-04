using System;
using UYCommerce.Models;

namespace UYCommerce.ViewModels
{
    public class SkuWishlistVM
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Key { get; set; }
        public int Stock { get; set; }
        public double Precio { get; set; }
        public double PrecioAnterior { get; set; }
        public ICollection<AtributoValor>? AtributosValores { get; set; }
        public ICollection<SkuImagen>? Imagenes { get; set; }
        public bool IsInCart { get; set; } = false;
    }
}

