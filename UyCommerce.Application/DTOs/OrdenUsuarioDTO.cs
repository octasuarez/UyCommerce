using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.DTOs
{
    public class OrdenUsuarioDTO
    {
        public required Orden Orden { get; set; }
        public required Usuario Usuario { get; set; }
    }
}
