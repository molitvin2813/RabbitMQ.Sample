using MediatR;
using OrderService.Application.Common.Response;
using OrderService.Application.Interfaces;
using OrderService.Domain.Models;
using SharedCollection.Interfaces;

namespace OrderService.Application.Mediator.Commands.BasketCommands.CreateBasket
{
    public class CreateBasketHandler
        : IRequestHandler<CreateBasketCommand, IServiceResponse>
    {
        public CreateBasketHandler(IOrderServiceContext context)
        {
            _context = context;
        }

        private readonly IOrderServiceContext _context;

        public async Task<IServiceResponse> Handle(
            CreateBasketCommand command,
            CancellationToken token)
        {
            var date = DateTime.Now;
            var basket = new Basket()
            {
                OrderId = command.OrderID,
                ProductId = command.ProductId,
                CreateDate = date,
                UpdateDate = date,
                Count = command.Count
            };

            await _context.Baskets
                .AddAsync(basket, token);

            await _context.SaveChangesAsync(token);

            return new ServiceResponseWrite<int>()
            {
                Key = basket.BasketId
            };
        }
    }
}
