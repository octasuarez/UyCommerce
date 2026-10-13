namespace UyCommerce.Application.DTOs
{
	public class CategoriaDTO
	{
        public string Nombre { get; set; }
        public bool MostrarEnInicio { get; set; } = false;
        public bool EsCategoriaPadre { get; set; } = false;
        public string? ImagenNombre {  get; set; }
        public int CategoriaPadreId { get; set; }
        public virtual ICollection<int>? Atributos { get; set; }
    }
}

