using MediatR;
using OrderService.Application.Common.Response;
using OrderService.Application.Interfaces;
using OrderService.Domain.Models;
using SharedCollection.Interfaces;

namespace OrderService.Application.Mediator.Commands.OrderCommands.CreateOrder
{
    public class CreateOrderHandler
        : IRequestHandler<CreateOrderCommand, IServiceResponse>
    {
        public CreateOrderHandler(IOrderServiceContext context)
        {
            _context = context;
        }

        private readonly IOrderServiceContext _context;

        public async Task<IServiceResponse> Handle(
            CreateOrderCommand command,
            CancellationToken token)
        {
            var data = new Order()
            {
                IsConfirmed = false,
                CrateDate = DateTime.Now,
                ConfirmedDate = null,
            };

            await _context.Orders.AddAsync(data, token);

            await _context.SaveChangesAsync(token);

            return new ServiceResponseWrite<Guid>()
            {
                Key = data.OrderId,
            };
        }
    }
}
