using MediatR;
using SharedCollection.Interfaces;

namespace OrderService.Application.Mediator.Commands.BasketCommands.CreateBasket
{
    public class CreateBasketCommand
        : IRequest<IServiceResponse>
    {
        public Guid OrderID { get; set; }
        public int ProductId { get; set; }

        public int Count { get; set; }
    }
}
