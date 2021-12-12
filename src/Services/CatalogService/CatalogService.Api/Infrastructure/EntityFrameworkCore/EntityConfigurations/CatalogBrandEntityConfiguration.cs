using CatalogService.Api.Domain;
using CatalogService.Api.Infrastructure.EntityFrameworkCore.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Api.Infrastructure.EntityFrameworkCore.EntityConfigurations
{
    public class CatalogBrandEntityConfiguration : IEntityTypeConfiguration<CatalogBrand>
    {
        public void Configure(EntityTypeBuilder<CatalogBrand> builder)
        {
            builder.ToTable(nameof(CatalogBrand), CatalogDbContext.DefaultSchema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
            .UseHiLo("catalog_brand_hilo")
            .IsRequired();

            builder.Property(x => x.Brand)
            .IsRequired()
            .HasMaxLength(100);
        }
    }
}