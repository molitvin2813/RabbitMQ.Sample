using CatalogService.Application.Common.Response;
using CatalogService.Application.Interfaces;
using MediatR;
using SharedCollection.Interfaces;

namespace CatalogService.Application.Mediator.Commands.ProductCommands.CreateProduct
{
    public class CreateProductHandler
        : IRequestHandler<CreateProductCommand, IServiceResponse>
    {
        public CreateProductHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private readonly IUnitOfWork _unitOfWork;

        public async Task<IServiceResponse> Handle(
            CreateProductCommand command,
            CancellationToken cancellationToken)
        {
            await _unitOfWork.ProductService
                .CreateProductAsync(command, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ServiceResponseWrite();
        }
    }
}
