using MediatR;
using SharedCollection.Interfaces;

namespace CatalogService.Application.Mediator.Commands.ProductCommands.DeleteProduct
{
    public class DeleteProductCommand
        : IRequest<IServiceResponse>
    {
        public int ProductId { get; set; }
    }
}
