using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OrderService.Application.Interfaces;
using OrderService.Domain.Models;
using OrderService.PostgreSQL.EntityTypeConfiguration;
using SharedCollection.Events;

namespace OrderService.PostgreSQL
{
    public class OrderServiceContext : DbContext, IOrderServiceContext
    {
        public OrderServiceContext() { }
        public OrderServiceContext(DbContextOptions<OrderServiceContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<EventOutbox> Events { get; set; }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken token)
        {
            return await Database.BeginTransactionAsync(token);
        }

        public async Task CommitTransactionAsync(CancellationToken token)
        {
            await Database.CommitTransactionAsync(token);
        }

        public async Task RollbackTransactionAsync(CancellationToken token)
        {
            await Database.RollbackTransactionAsync(token);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new EventOutboxConfiguration());
            builder.ApplyConfiguration(new OrderConfiguration());
            builder.ApplyConfiguration(new BasketConfiguration());

            base.OnModelCreating(builder);
        }
    }
}
