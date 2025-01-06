using MediatR;
using SharedCollection.Interfaces;

namespace OrderService.Application.Mediator.Queries.BasketQueries
{
    public class GetBasketByOrderIdQuery
        : IRequest<IServiceResponse>
    {
        public Guid OrderId { get; set; }
    }
}
