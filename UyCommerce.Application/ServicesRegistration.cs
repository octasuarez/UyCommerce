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
            services.AddScoped<IProductoService,ProductoService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IImagesService, ImagesService>();
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<IOrdenService, OrdenService>();
            return services;
        }
    }
}
