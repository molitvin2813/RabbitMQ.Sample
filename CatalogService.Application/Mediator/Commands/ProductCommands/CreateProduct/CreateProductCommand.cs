using MediatR;
using SharedCollection.Interfaces;

namespace CatalogService.Application.Mediator.Commands.ProductCommands.CreateProduct
{
    public class CreateProductCommand
        : IRequest<IServiceResponse>
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Count { get; set; }
    }
}
