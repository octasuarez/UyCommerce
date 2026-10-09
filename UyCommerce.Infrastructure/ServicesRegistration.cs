using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UyCommerce.Infrastructure.Persistance;

namespace UyCommerce.Infrastructure
{
    public static class ServicesRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString =  configuration.GetConnectionString("Default");
            services.AddDbContext<ShopContext>(options => options.UseMySql(
            connectionString, ServerVersion.AutoDetect(connectionString)));

            return services;
        }
    }
}
