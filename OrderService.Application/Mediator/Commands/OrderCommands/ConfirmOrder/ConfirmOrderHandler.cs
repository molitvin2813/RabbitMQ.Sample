using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OrderService.Application.Common.Response;
using OrderService.Application.Interfaces;
using OrderService.Domain.Models;
using SharedCollection.Enums;
using SharedCollection.Events;
using SharedCollection.Exceptions;
using SharedCollection.Interfaces;

namespace OrderService.Application.Mediator.Commands.OrderCommands.ConfirmOrder
{
    public class ConfirmOrderHandler
        : IRequestHandler<ConfirmOrderCommand, IServiceResponse>
    {
        public ConfirmOrderHandler(IOrderServiceContext context)
        {
            _context = context;
        }

        private readonly IOrderServiceContext _context;

        public async Task<IServiceResponse> Handle(
            ConfirmOrderCommand command,
            CancellationToken token)
        {
            using (var transaction = await _context.BeginTransactionAsync(token))
            {
                var order = await _context.Orders
                    .Include(x => x.Baskets)
                    .FirstOrDefaultAsync(x => x.OrderId == command.OrderId);

                if (order == null)
                {
                    transaction.Rollback();
                    throw new RecordNotFoundException<Guid>(nameof(Order), command.OrderId);
                }

                order.IsConfirmed = true;
                order.ConfirmedDate = DateTime.Now;

                foreach (var items in order.Baskets)
                {
                    var integrationEvent = new OrderConfirmedEvent()
                    {
                        IntegrationEventId = Guid.NewGuid(),
                        ProductId = items.ProductId,
                        UnitSold = items.Count
                    };

                    await _context.Events.AddAsync(new EventOutbox()
                    {
                        EventDTO = JsonConvert.SerializeObject(integrationEvent),
                        EventType = integrationEvent.GetType().AssemblyQualifiedName,
                        PublishStatus = EventPublishStatusEnum.Unpublished,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        IsIncomeEvent = false,
                        EventOutboxId = integrationEvent.IntegrationEventId,
                    }, token);
                }

                await _context.SaveChangesAsync(token);
                transaction.Commit();
            }

            return new ServiceResponseWrite<Guid>()
            {
                Key = command.OrderId,
            };
        }
    }
}
