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
        }
    }
}
