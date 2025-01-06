using MediatR;
using SharedCollection.Interfaces;

namespace OrderService.Application.Mediator.Commands.BasketCommands.UpdateBasket
{
    public class UpdateBasketCommand
        : IRequest<IServiceResponse>
    {
        public int ProductId { get; set; }
    }
}
