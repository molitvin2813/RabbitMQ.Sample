using MediatR;
using SharedCollection.Interfaces;

namespace OrderService.Application.Mediator.Commands.OrderCommands.ConfirmOrder
{
    public class ConfirmOrderCommand
        : IRequest<IServiceResponse>
    {
        public Guid OrderId { get; set; }
    }
}
