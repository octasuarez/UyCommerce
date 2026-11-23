namespace UyCommerce.Application.DTOs.Carritos
{
    public class GetCarritoCompleteByUserIdResponse
    {
        public double Total { get; set; }
        public List<GetCarritoCompleteByUserIdProductoCarritoDto> Productos { get; set; } = [];
    }
    public class GetCarritoCompleteByUserIdProductoCarritoDto
    {
        public int SkuId { get; set; }
        public required GetCarritoCompleteByUserIdSkuDto Sku { get; set; }
        public int Cantidad { get; set; }
        public string Total { get; set; } = string.Empty;

    }
    public class GetCarritoCompleteByUserIdSkuDto
    {
        public required string Nombre { get; set; }
        public required string Key { get; set; }
        public double Precio { get; set; }
        public List<GetCarritoCompleteByUserIdAtributoValorDto> AtributosValores { get; set; } = [];
        public List<GetCarritoCompleteByUserIdSkuImagenDto> Imagenes { get; set; } = [];
    }
    public class GetCarritoCompleteByUserIdAtributoValorDto
    {
        public required GetCarritoCompleteByUserIdAtributoDto Atributo { get; set; }
        public string Valor { get; set; } = string.Empty;
    }

    public class GetCarritoCompleteByUserIdSkuImagenDto
    {
        public string ImagenNombre { get; set; } = string.Empty;
    }

    public class GetCarritoCompleteByUserIdAtributoDto
    {
        public string Nombre { get; set; } = string.Empty;
    }
}