using EventBus.Abstraction.Interfaces;
using Microsoft.Extensions.Logging;
using OrderService.Application.Interfaces;
using SharedCollection.Events;

namespace OrderService.Application.RabbitMQ
{
    public class ProductUpdateHandler
        : IIntegrationEventHandler<ProductUpdateEvent>
    {
        public ProductUpdateHandler(
            IOrderServiceContext context,
            ILogger<OrderConfirmedEvent> logger)
        {
            _context = context;
            _logger = logger;
        }

        private readonly IOrderServiceContext _context;
        private readonly ILogger<OrderConfirmedEvent> _logger;

        public async Task Handle(
            ProductUpdateEvent integrationEvent,
            CancellationToken token)
        {
        }
    }
}
