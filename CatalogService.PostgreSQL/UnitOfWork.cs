using AutoMapper;
using CatalogService.Application.Interfaces;
using CatalogService.PostgreSQL.Services;
using EventBus.Abstraction.Interfaces;

namespace CatalogService.PostgreSQL
{
    public class UnitOfWork
        : IUnitOfWork
    {
        public UnitOfWork(
            CatalogServiceContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private readonly CatalogServiceContext _context;
        private readonly IMapper _mapper;

        private IProductService _productService;
        private IIntegrationEventOutboxService _integrationEventOutboxService;
        private bool _disposed;

        public IProductService ProductService
            => _productService ??= new ProductService(_mapper, _context);

        public IIntegrationEventOutboxService IntegrationEventOutboxService
            => _integrationEventOutboxService ??= new IntegrationEventOutboxService(_context);

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            await _context.Database.CommitTransactionAsync(cancellationToken);
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
        {
            await _context.Database.RollbackTransactionAsync(cancellationToken);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _context.Database.CurrentTransaction?.Dispose();
                _context.Dispose();
                _disposed = true;
            }
        }
    }
}
