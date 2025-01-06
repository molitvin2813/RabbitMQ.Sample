using MediatR;
using SharedCollection.Interfaces;

namespace CatalogService.Application.Mediator.Commands.ProductCommands.UpdateProduct
{
    public class UpdateProductCommand
        : IRequest<IServiceResponse>
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }

        public decimal? Price { get; set; }

        public int? Count { get; set; }
    }
}
