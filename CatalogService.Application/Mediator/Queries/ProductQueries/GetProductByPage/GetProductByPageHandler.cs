using CatalogService.Application.Common.DTO.ProductDTO;
using CatalogService.Application.Common.Response;
using CatalogService.Application.Interfaces;
using MediatR;
using SharedCollection.Interfaces;

namespace CatalogService.Application.Mediator.Queries.ProductQueries.GetProductByPage
{
    public class GetProductByPageHandler
        : IRequestHandler<GetProductByPageQuery, IServiceResponse>
    {

        public GetProductByPageHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private readonly IUnitOfWork _unitOfWork;

        public async Task<IServiceResponse> Handle(
            GetProductByPageQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.ProductService
                .GetProductByPageAsync(query.Take, query.Page, cancellationToken);

            return new ServiceResponseRead<List<GetProductDTO>>()
            {
                Data = result
            };
        }
    }
}
