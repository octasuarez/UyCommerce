﻿using System;
namespace UYCommerce.Models
{
	public class Sku
	{
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public virtual Producto? Producto { get; set; }
        public required string Nombre { get; set; }
        public required string Key { get; set; }
        public int Stock { get; set; }
        public double Precio { get; set; }
        public double PrecioAnterior { get; set; }
        public ICollection<AtributoValor>? AtributosValores { get; set; }
        public ICollection<SkuImagen>? Imagenes { get; set; }
        public virtual ICollection<Usuario>? Usuarios { get; set; }
    }
}

