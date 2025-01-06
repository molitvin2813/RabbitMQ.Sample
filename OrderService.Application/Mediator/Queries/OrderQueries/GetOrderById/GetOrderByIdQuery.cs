using MediatR;
using SharedCollection.Interfaces;

namespace OrderService.Application.Mediator.Queries.OrderQueries.GetOrderById
{
    public class GetOrderByIdQuery
        : IRequest<IServiceResponse>
    {
        public Guid OrderId { get; set; }
    }
}
