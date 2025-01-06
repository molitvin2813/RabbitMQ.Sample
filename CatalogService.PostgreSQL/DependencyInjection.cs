using CatalogService.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CatalogService.PostgreSQL
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddPostgresDB(this IServiceCollection services, IConfiguration configuration)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddDbContext<CatalogServiceContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("CatalogServiceContext")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
