using System;
using System.Collections.Generic;
using System.Text;

namespace UyCommerce.Application.DTOs
{
    public class CreateImageDto
    {
        public string ImageName { get; set; } = string.Empty;
        public Stream? Stream { get; set; }
    }
}
