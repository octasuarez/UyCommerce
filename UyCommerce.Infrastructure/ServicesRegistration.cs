using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Application.Contracts.Persistance.Services;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Infrastructure.Persistance;
using UyCommerce.Infrastructure.Persistance.Repositories;
using UyCommerce.Infrastructure.Services.Files;

namespace UyCommerce.Infrastructure
{
    public static class ServicesRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString =  configuration.GetConnectionString("Default");
            services.AddDbContext<ShopContext>(options => options.UseMySql(
            connectionString, ServerVersion.AutoDetect(connectionString)));

            services.AddSingleton<IFileService, FileService>();

            services.AddScoped<IAtributoRepository, AtributoRepository>();
            services.AddScoped<ICarouselImageRepository, CarouselImageRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
            services.AddScoped<IOrdenRepository, OrdenRepository>();
            services.AddScoped<IPreguntaRepository, PreguntaRepository>();
            services.AddScoped<IProductoRepository, ProductoRepository>();
            services.AddScoped<IReviewRepository,ReviewRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            return services;
        }
    }
}
