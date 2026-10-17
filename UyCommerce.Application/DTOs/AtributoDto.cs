using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.DTOs
{
    public class AtributoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public List<string> Valores { get; set; } = [];
    }
}
