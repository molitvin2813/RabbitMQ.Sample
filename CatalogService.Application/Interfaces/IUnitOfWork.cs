using EventBus.Abstraction.Interfaces;

namespace CatalogService.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public IProductService ProductService { get; }

        public IIntegrationEventOutboxService IntegrationEventOutboxService { get; }

        public Task SaveChangesAsync(CancellationToken cancellationToken);

        public Task BeginTransactionAsync(CancellationToken cancellationToken);

        public Task CommitTransactionAsync(CancellationToken cancellationToken);

        public Task RollbackTransactionAsync(CancellationToken cancellationToken);
    }
}
