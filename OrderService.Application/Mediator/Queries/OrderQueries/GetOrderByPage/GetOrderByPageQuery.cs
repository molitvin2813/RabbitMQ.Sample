using MediatR;
using SharedCollection.Interfaces;

namespace OrderService.Application.Mediator.Queries.OrderQueries.GetOrderByPage
{
    public class GetOrderByPageQuery
        : IRequest<IServiceResponse>
    {
        public int Take { get; set; }
        public int Page { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
