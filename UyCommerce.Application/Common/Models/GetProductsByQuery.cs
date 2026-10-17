using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Common.Models
{
    public class GetProductsByQuery
    {
        public List<Sku> Items = new List<Sku>();
        public List<Categoria> Categorias = new List<Categoria>();
        public List<Sku> Favoritos { get; set; } = new List<Sku>();
        public int TotalItems { get; set; }
        public int Pages { get; set; }
    }
}
