namespace UyCommerce.Application.DTOs.Categorias
{
    public class GetCategoriasCompleteResponse
    {
        public List<GetCategoriasCompleteDto> Categorias { get; set; } = [];
    }

    public class GetCategoriasCompleteDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public virtual List<GetCategoriasCompleteProductoDto> Productos { get; set; } = [];
        public bool MostrarEnInicio { get; set; }
        public string ImagenNombre { get; set; } = string.Empty;
        public virtual List<GetCategoriasCompleteAtributoDto> Atributos { get; set; } = [];
    }
    public class GetCategoriasCompleteProductoDto
    {
        public int Id { get; set; }
    }
    public class GetCategoriasCompleteAtributoDto
    {
        public int Id { get; set; }
    }
}
