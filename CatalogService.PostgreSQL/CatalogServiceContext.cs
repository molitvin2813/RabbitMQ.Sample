using CatalogService.Domain.Models;
using CatalogService.PostgreSQL.EntityTypeConfiguration;
using Microsoft.EntityFrameworkCore;
using SharedCollection.Events;

namespace CatalogService.PostgreSQL
{
    public class CatalogServiceContext : DbContext
    {
        public CatalogServiceContext() { }
        public CatalogServiceContext(DbContextOptions<CatalogServiceContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<EventOutbox> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new EventOutboxConfiguration());
            builder.ApplyConfiguration(new ProductConfiguration());

            base.OnModelCreating(builder);
        }
    }
}
