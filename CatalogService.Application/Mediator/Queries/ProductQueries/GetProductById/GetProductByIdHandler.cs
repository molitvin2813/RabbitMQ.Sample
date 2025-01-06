using CatalogService.Application.Common.DTO.ProductDTO;
using CatalogService.Application.Common.Response;
using CatalogService.Application.Interfaces;
using MediatR;
using SharedCollection.Interfaces;

namespace CatalogService.Application.Mediator.Queries.ProductQueries.GetProductById
{
    public class GetProductByIdHandler
        : IRequestHandler<GetProductByIdQuery, IServiceResponse>
    {
        public GetProductByIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private readonly IUnitOfWork _unitOfWork;

        public async Task<IServiceResponse> Handle(
            GetProductByIdQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.ProductService
                .GetProductByIdAsync(query.ProductId, cancellationToken);

            return new ServiceResponseRead<GetProductDTO>()
            {
                Data = result,
            };
        }
    }
}
