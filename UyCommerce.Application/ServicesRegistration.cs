using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Application.Services;

namespace UyCommerce.Application
{
    public static class ServicesRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAtributoService, AtributoService>();
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<IProductoService,ProductoService>();
            services.AddScoped<IImagesService, ImagesService>();
            services.AddScoped<IOrdenService, OrdenService>();
            services.AddScoped<IPreguntaService,PreguntaService>();
            services.AddScoped<IProductoService,ProductoService>();
            services.AddScoped<IReviewService,ReviewService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<ICarritoService, CarritoService>();
            return services;
        }
    }
}
