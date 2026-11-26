
namespace UyCommerce.Application.DTOs.Categorias
{
    public class GetCategoriaAtributosResponse
    {
        public List<GetCategoriaAtributosAtributoDto> Atributos { get; set; } = [];
    }

    public class GetCategoriaAtributosAtributoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
