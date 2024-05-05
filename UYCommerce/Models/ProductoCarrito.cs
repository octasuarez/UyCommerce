using System;
using Microsoft.EntityFrameworkCore;

namespace UYCommerce.Models
{
    [PrimaryKey(nameof(CarritoId), nameof(SkuId))]
    public class ProductoCarrito
    {
        public int CarritoId { get; set; }
        public int SkuId { get; set; }
        public virtual Sku? Sku { get; set; }
        public int Cantidad { get; set; }


        public double GetTotal()
        {
            if (Sku is null) return 0;
            return Sku.Precio * Cantidad;
        }

        public string GetTotalString()
        {
            return GetTotal().ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}

