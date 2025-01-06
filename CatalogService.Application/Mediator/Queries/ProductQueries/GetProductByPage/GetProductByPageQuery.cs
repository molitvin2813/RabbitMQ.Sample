using MediatR;
using SharedCollection.Interfaces;

namespace CatalogService.Application.Mediator.Queries.ProductQueries.GetProductByPage
{
    public class GetProductByPageQuery
        : IRequest<IServiceResponse>
    {
        public int Take { get; set; }

        public int Page { get; set; }
    }
}
