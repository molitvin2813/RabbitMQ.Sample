using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Models;

namespace OrderService.PostgreSQL.EntityTypeConfiguration
{
    internal class OrderConfiguration : IEntityTypeConfiguration<Order>
    {

        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.OrderId);

            builder.Property(x => x.IsConfirmed)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.CrateDate)
                .IsRequired();

            builder.Property(x => x.ConfirmedDate)
                .IsRequired(false);

            builder.HasIndex(x => x.CrateDate);
            builder.HasIndex(x => x.ConfirmedDate);

        }
    }
}
