using System;
using System.ComponentModel.DataAnnotations;

namespace UYCommerce.Models
{
    public class Compra
    {
        [Key]
        public required string CompraId { get; set; }
        public int UsuarioId { get; set; }
        public string? FechaDeCompra { get; set; }
        public List<ProductoCompra>? Productos { get; set; } = new List<ProductoCompra>();
    }
}

