using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OrderService.Domain.Models;
using SharedCollection.Events;

namespace OrderService.Application.Interfaces
{
    public interface IOrderServiceContext
    {
        DbSet<Order> Orders { get; set; }
        DbSet<Basket> Baskets { get; set; }
        DbSet<EventOutbox> Events { get; set; }

        Task<int> SaveChangesAsync(CancellationToken token);
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken token);
        Task CommitTransactionAsync(CancellationToken token);
        Task RollbackTransactionAsync(CancellationToken token);
    }
}
