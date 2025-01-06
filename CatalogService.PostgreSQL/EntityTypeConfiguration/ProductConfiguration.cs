using CatalogService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.PostgreSQL.EntityTypeConfiguration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.ProductId);

            builder.HasIndex(x => x.CreatedDate);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Price)
                .IsRequired();

            builder.Property(x => x.Count)
                .IsRequired();

            builder.Property(x => x.CreatedDate)
                .IsRequired();
        }
    }
}
