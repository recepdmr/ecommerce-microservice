using CatalogService.Api.Infrastructure.EntityFrameworkCore.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Api.Extensions
{
    public static class DbContextExtensions
    {
        public static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<CatalogDbContextSeeder>();

            services.AddDbContext<CatalogDbContext>(options =>
                {
                    options.UseNpgsql(configuration.GetConnectionString("Default"), postgreSql =>
                    {
                        postgreSql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
                        postgreSql.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
                    });
                });

            return services;
        }
    }
}