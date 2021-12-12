using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CatalogService.Api.Infrastructure.EntityFrameworkCore.DbContexts
{
    public class CatalogDbContexxFactory : IDesignTimeDbContextFactory<CatalogDbContext>
    {
        public CatalogDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CatalogDbContext>();


            var connectionString = "Server=localhost;Port=5432;Database=catalog;User Id=postgres;Password=password;";
            optionsBuilder.UseNpgsql(connectionString);

            return new CatalogDbContext(optionsBuilder.Options);
        }
    }
}