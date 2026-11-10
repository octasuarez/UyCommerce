using System;
using System.Collections.Generic;
using System.Text;

namespace UyCommerce.Application.DTOs.Productos
{
    public class GetProductosByQueryNameResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string NombreClave { get; set; }
        public string ImagenNombre { get; set; }
    }
}
