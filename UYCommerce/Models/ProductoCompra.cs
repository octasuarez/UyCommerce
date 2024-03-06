using System;
using Microsoft.EntityFrameworkCore;

namespace UYCommerce.Models
{
    [PrimaryKey(nameof(CompraId), nameof(SkuId))]
    public class ProductoCompra
    {

        public required string CompraId { get; set; }
        public int SkuId { get; set; }
        public decimal PrecioComprado { get; set; }
        public int Cantidad { get; set; }

        public virtual Sku? Sku { get; set; }
    }
}

