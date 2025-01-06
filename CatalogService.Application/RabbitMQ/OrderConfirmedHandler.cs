using CatalogService.Application.Interfaces;
using CatalogService.Application.Mediator.Commands.ProductCommands.UpdateProduct;
using EventBus.Abstraction.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SharedCollection.Enums;
using SharedCollection.Events;

namespace CatalogService.Application.RabbitMQ
{
    public class OrderConfirmedHandler
        : IIntegrationEventHandler<OrderConfirmedEvent>
    {
        public OrderConfirmedHandler(
            IUnitOfWork unitOfWork,
            ILogger<OrderConfirmedEvent> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderConfirmedEvent> _logger;

        public async Task Handle(
            OrderConfirmedEvent integrationEvent,
            CancellationToken token)
        {
            try
            {
                var eventEntry = await _unitOfWork.IntegrationEventOutboxService
                    .CheckEntryEvent(integrationEvent.IntegrationEventId, token);

                if (!eventEntry)
                {
                    await _unitOfWork.IntegrationEventOutboxService
                        .SaveEventAsync(integrationEvent, true, token);

                    await _unitOfWork.SaveChangesAsync(token);
                }

                await _unitOfWork.BeginTransactionAsync(token);

                await _unitOfWork.IntegrationEventOutboxService
                   .ChangeEventProcessingStatusAsync(
                       integrationEvent.IntegrationEventId,
                       null,
                       EventProcessingStatusEnum.InProgress,
                       token
                   );

                var data = await _unitOfWork.ProductService
                    .GetProductByIdAsync(integrationEvent.ProductId, token);

                await _unitOfWork.ProductService.UpdateProductAsync(new UpdateProductCommand()
                {
                    ProductId = data.ProductId,
                    Count = data.Count - integrationEvent.UnitSold
                }, token);

                await _unitOfWork.IntegrationEventOutboxService
                   .ChangeEventProcessingStatusAsync(
                       integrationEvent.IntegrationEventId,
                       null,
                       EventProcessingStatusEnum.Processed,
                       token
                   );

                await _unitOfWork.SaveChangesAsync(token);
                await _unitOfWork.CommitTransactionAsync(token);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(token);
                _logger.LogError("Произошла ошибка при обработке Event {Id} \\n{IntegrationEvent}\\nОшибка: {error}",
                    integrationEvent.IntegrationEventId,
                    JsonConvert.SerializeObject(integrationEvent),
                    ex.Message);

                await _unitOfWork.IntegrationEventOutboxService
                    .ChangeEventProcessingStatusAsync(
                        integrationEvent.IntegrationEventId,
                        null,
                        EventProcessingStatusEnum.ProcessingFailed,
                        token
                    );

                await _unitOfWork.SaveChangesAsync(token);

            }
        }
    }
}
