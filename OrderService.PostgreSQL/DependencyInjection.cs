using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Interfaces;

namespace OrderService.PostgreSQL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPostgresDB(this IServiceCollection services, IConfiguration configuration)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddDbContext<OrderServiceContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("CatalogServiceContext")));

            services.AddScoped<IOrderServiceContext, OrderServiceContext>();
            return services;
        }
    }
}

