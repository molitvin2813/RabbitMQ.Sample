using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Models;

namespace OrderService.PostgreSQL.EntityTypeConfiguration
{
    public class BasketConfiguration : IEntityTypeConfiguration<Basket>
    {

        public void Configure(EntityTypeBuilder<Basket> builder)
        {
            builder.HasKey(x => x.BasketId);

            builder.Property(x => x.OrderId)
                .IsRequired();

            builder.Property(x => x.ProductId)
                .IsRequired();

            //builder.Property(x => x.Name)
            //    .IsRequired();

            //builder.Property(x => x.Price)
            //    .IsRequired();

            builder.Property(x => x.CreateDate)
                .IsRequired();

            builder.Property(x => x.UpdateDate)
                .IsRequired();

            builder.HasOne(x => x.Order)
                .WithMany(y => y.Baskets)
                .HasForeignKey(x => x.OrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
