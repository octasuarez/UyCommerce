using Mapster;
using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.DTOs.Productos;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Mappings
{
    public class MappingProfiles : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Sku, GetProductosSkuDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Nombre, src => src.Nombre)
                .Map(dest => dest.Key, src => src.Key)
                .Map(dest => dest.Precio, src => src.Precio)
                .Map(dest => dest.PrecioAnterior, src => src.PrecioAnterior)
                .Map(dest => dest.Puntuacion, src => src.Producto.GetPuntuacionPromedio())
                .Map(dest => dest.Imagenes, src => src.Imagenes.Select(i => i.ImagenNombre));

            config.NewConfig<Categoria, GetProductosCategoriaDto>();
            config.NewConfig<Categoria, GetProductosCategoriaSelectedDto>();

            config.NewConfig<AtributoValor, GetSkuByKeyAtributoValorDto>()
                .Map(dest => dest.Atributo, src => src.Atributo.Nombre)
                .Map(dest => dest.Valor, src => src.Valor);

            config.NewConfig<Review, GetSkuByKeyReviewDto>()
                .Map(dest => dest.UsuarioNombre, src => src.Usuario.Nombre)
                .Map(dest => dest.Puntuacion, src => src.Puntuacion)
                .Map(dest => dest.Texto, src => src.Texto);

            config.NewConfig<Producto, GetSkuByKeyProductoDto>()
                .Map(dest => dest.Puntuacion, src => src.GetPuntuacionPromedio());

            config.NewConfig<Sku, GetSkuByKeySkuDto>()
                .Map(dest => dest.Imagenes, src => src.Imagenes.Select(i => i.ImagenNombre));

            config.NewConfig<Pregunta, GetSkuByKeyPreguntaDto>()
                .Map(dest => dest.NombreUsuario, src => src.Usuario.Nombre)
                .Map(dest => dest.Fecha, src => src.Fecha)
                .Map(dest => dest.Contenido, src => src.Contenido)
                .Map(dest => dest.Respuesta, src => src.Respuesta.Contenido);

            config.NewConfig<Categoria, GetSkuByKeyCategoriaDto>()
                .Map(dest => dest.Nombre, src => src.Nombre)
                .Map(dest => dest.CategoriaPadreNombre, src => src.CategoriaPadre != null ? src.CategoriaPadre.Nombre : string.Empty);
        }
    }
}
