using CatalogService.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace CatalogService.Api.Infrastructure.EntityFrameworkCore.DbContexts;

public class CatalogDbContextSeeder
{
    public CatalogDbContextSeeder(IServiceProvider serviceProvider, ILogger<CatalogDbContextSeeder> logger)
    {
        ServiceProvider = serviceProvider;
        Logger = logger;
    }

    private CatalogDbContext CatalogDbContext { get; set; }
    private IServiceProvider ServiceProvider { get; }
    private ILogger<CatalogDbContextSeeder> Logger { get; }

    public async Task SeedAsync()
    {
        using (var scope = ServiceProvider.CreateScope())
        {
            CatalogDbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            var policy = Policy.Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                onRetry: (exception, timeSpan, retry, ctx) =>
                {
                    Logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of ");
                }
            );

            await policy.ExecuteAsync(async () =>
            {

                await CatalogDbContext.Database.EnsureCreatedAsync();
                await CatalogDbContext.Database.MigrateAsync();
                var brands = new List<CatalogBrand>
                    {
                    new CatalogBrand
                    {
                        Id=1,
                        Brand="Azure"
                    },
                    new CatalogBrand
                    {
                        Id=2,
                        Brand=".Net"
                    },
                    new CatalogBrand
                    {
                        Id=3,
                        Brand="Visual Studio"
                    },
                    new CatalogBrand
                    {
                        Id=4,
                        Brand="PostgreSQL"
                    },
                    new CatalogBrand
                    {
                        Id=5,
                        Brand="Other"
                    }
                    };
                if (!await CatalogDbContext.CatalogBrands.AnyAsync())
                {
                    await CatalogDbContext.CatalogBrands.AddRangeAsync(brands);
                    await CatalogDbContext.SaveChangesAsync();
                }

                var types = new List<CatalogType>
                    {
                    new CatalogType
                    {
                        Id=1,
                        Type="Mug"
                    },
                    new CatalogType
                    {
                        Id=2,
                        Type="T-Shirt"
                    },
                    new CatalogType
                    {
                        Id=3,
                        Type="Sheet"
                    },
                    new CatalogType
                    {
                        Id=4,
                        Type="USB Memory Stick"
                    }
                    };
                if (!await CatalogDbContext.CatalogTypes.AnyAsync())
                {
                    await CatalogDbContext.CatalogTypes.AddRangeAsync(types);

                    await CatalogDbContext.SaveChangesAsync();

                }

                if (!await CatalogDbContext.CatalogItems.AnyAsync())
                {
                    var catalogItems = new List<CatalogItem>
                    {
                    new CatalogItem
                    {
                        Id=1,
                        CatalogBrandId = brands[1].Id,
                        Name=".NET Bot Black Hoodie",
                        Description=".NET Bot Black Hoodie Desc",
                        PictureFileName="https://picsum.photos/200/300?random=2",
                        Price= 18.3M,
                        CatalogTypeId = types[1].Id
                    },
                    new CatalogItem
                    {
                        Id=2,
                        CatalogBrandId = brands[0].Id,
                        CatalogTypeId = types[0].Id,
                        Name=".Net Mug",
                        Description=".Net Mug Desc",
                        PictureFileName="https://picsum.photos/200/300?random=2",
                        Price=3.4M
                    },
                    new CatalogItem
                    {
                        Id=3,
                        CatalogBrandId = brands[3].Id,
                        CatalogTypeId = types[2].Id,
                        Name="Sheet Item",
                        Description="Sheet Item Desc",
                        PictureFileName="https://picsum.photos/200/300?random=2",
                        Price=3.4M
                    }
                    };

                    await CatalogDbContext.CatalogItems.AddRangeAsync(catalogItems);
                    await CatalogDbContext.SaveChangesAsync();
                }
            });
        }
    }
}