using MediatR;
using SharedCollection.Interfaces;

namespace CatalogService.Application.Mediator.Queries.ProductQueries.GetProductById
{
    public class GetProductByIdQuery
        : IRequest<IServiceResponse>
    {
        public int ProductId { get; set; }
    }
}
