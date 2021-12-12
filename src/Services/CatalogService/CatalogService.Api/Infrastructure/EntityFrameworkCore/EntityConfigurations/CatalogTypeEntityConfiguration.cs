using CatalogService.Api.Domain;
using CatalogService.Api.Infrastructure.EntityFrameworkCore.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Api.Infrastructure.EntityFrameworkCore.EntityConfigurations
{
    public class CatalogTypeEntityConfiguration : IEntityTypeConfiguration<CatalogType>
    {
        public void Configure(EntityTypeBuilder<CatalogType> builder)
        {
            builder.ToTable(nameof(CatalogType), CatalogDbContext.DefaultSchema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .UseHiLo("catalog_type_hilo")
                .IsRequired();

            builder.Property(x => x.Type)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}