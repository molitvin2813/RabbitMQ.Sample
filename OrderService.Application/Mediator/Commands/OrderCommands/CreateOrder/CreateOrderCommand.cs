using MediatR;
using SharedCollection.Interfaces;

namespace OrderService.Application.Mediator.Commands.OrderCommands.CreateOrder
{
    public class CreateOrderCommand
        : IRequest<IServiceResponse>
    {
    }
}
