using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCollection.Events;

namespace CatalogService.PostgreSQL.EntityTypeConfiguration
{
    public class EventOutboxConfiguration
        : IEntityTypeConfiguration<EventOutbox>
    {

        public void Configure(EntityTypeBuilder<EventOutbox> builder)
        {
            builder.HasKey(x => x.EventOutboxId);

            builder.HasIndex(x => x.CreateDate);
            builder.HasIndex(x => x.PublishStatus);
            builder.HasIndex(x => x.ProcessingStatus);
            builder.HasIndex(x => x.EventType);

            builder.Property(x => x.EventDTO)
                .IsRequired()
                .HasColumnType("jsonb");

            builder.Property(x => x.EventType)
                .IsRequired();

            builder.Property(x => x.PublishStatus)
                .IsRequired(false)
                .HasConversion<int>();

            builder.Property(x => x.ProcessingStatus)
                .IsRequired(false)
                .HasConversion<int>();

            builder.Property(x => x.IsIncomeEvent)
                .IsRequired();

            builder.Property(x => x.CreateDate)
                .IsRequired();

            builder.Property(x => x.UpdateDate)
                .IsRequired();
        }
    }
}