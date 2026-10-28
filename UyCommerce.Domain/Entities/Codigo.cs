using System;
using Microsoft.EntityFrameworkCore;

namespace UyCommerce.Domain.Entities
{
    [PrimaryKey(nameof(CodigoRecuperacion), nameof(UsuarioId))]
    public class Codigo
	{
		public string CodigoRecuperacion { get; set; }
		public int UsuarioId { get; set; }
		public DateTime FechaExpiracion { get; set; }

		public Codigo(string codigo, int usuarioId, DateTime fechaExpiracion)
		{
			CodigoRecuperacion = codigo;
			UsuarioId = usuarioId;
			FechaExpiracion = fechaExpiracion;
		}

		public Codigo() { }
	}
}

