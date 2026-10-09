using System;
using System.Collections.Generic;
using System.Text;

namespace UyCommerce.Domain.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTime FechaCreado { get; set; }
        public DateTime FechaActualizado { get; set; }
    }
}
