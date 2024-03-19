using System;
using Microsoft.EntityFrameworkCore;

namespace UYCommerce.Models
{
    [PrimaryKey(nameof(CodigoRecuperacion), nameof(UsuarioId))]
    public class Codigo
	{
		public string CodigoRecuperacion { get; set; }
		public int UsuarioId { get; set; }
		public DateTime FechaExpiracion { get; set; }

		public Codigo(string codigo, int usuarioId)
		{
			CodigoRecuperacion = codigo;
			UsuarioId = usuarioId;
			FechaExpiracion = DateTime.Now.AddDays(2);
		}

		public Codigo() { }
	}
}

