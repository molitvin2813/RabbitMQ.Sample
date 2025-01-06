using CatalogService.Application.Common.Response;
using CatalogService.Application.Interfaces;
using MediatR;
using SharedCollection.Interfaces;

namespace CatalogService.Application.Mediator.Commands.ProductCommands.DeleteProduct
{
    public class DeleteProductHandler
        : IRequestHandler<DeleteProductCommand, IServiceResponse>
    {
        public DeleteProductHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private readonly IUnitOfWork _unitOfWork;

        public async Task<IServiceResponse> Handle(
            DeleteProductCommand command,
            CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            await _unitOfWork.ProductService
                .DeleteProductAsync(command.ProductId, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return new ServiceResponseWrite();
        }
    }
}
