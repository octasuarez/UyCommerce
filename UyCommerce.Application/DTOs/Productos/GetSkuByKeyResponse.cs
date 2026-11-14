using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.DTOs.Productos
{
    public class GetSkuByKeyResponse
    {
        public GetSkuByKeySkuDto Sku { get; set; } = new();
        public List<AtributoDto> Opciones { get; set; } = [];
        public List<GetSkuByKeySkuDto> ProductosRelacionados { get; set; } = [];
        public GetSkuByKeyCategoriaDto Categoria { get; set; } = new();
    }

    public class GetSkuByKeySkuDto
    {
        public int Id { get; set; }
        public  GetSkuByKeyProductoDto Producto { get; set; } = new();
        public string Nombre { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public int Stock { get; set; }
        public double Precio { get; set; }
        public List<GetSkuByKeyAtributoValorDto> AtributosValores { get; set; } = [];
        public List<string> Imagenes { get; set; } = [];
        public bool EsFavorito { get; set; } = false;
    }

    public class GetSkuByKeyProductoDto
    {
        public int Id { get; set; }
        public string NombreClave { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int VecesComprado { get; set; }
        public double Puntuacion { get; set; }
        public List<GetSkuByKeyReviewDto> Reviews { get; set; } = [];
        public List<GetSkuByKeyPreguntaDto> Preguntas { get; set; } = [];

    }

    public class GetSkuByKeyAtributoValorDto
    {
        public string Atributo { get; set; } = string.Empty;
        public string Valor { get; set; } = string.Empty;
    }

    public class GetSkuByKeyReviewDto
    {
        public string UsuarioNombre { get; set; } = string.Empty;
        public string Texto { get; set; } = string.Empty;
        public int Puntuacion { get; set; }
    }

    public class GetSkuByKeyPreguntaDto
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string Contenido { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Respuesta { get; set; } = string.Empty;
    }

    public class GetSkuByKeyCategoriaDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string CategoriaPadreNombre { get; set; } = string.Empty;
    } 
}
