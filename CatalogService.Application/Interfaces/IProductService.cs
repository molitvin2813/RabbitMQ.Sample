using CatalogService.Application.Common.DTO.ProductDTO;
using CatalogService.Application.Mediator.Commands.ProductCommands.CreateProduct;
using CatalogService.Application.Mediator.Commands.ProductCommands.UpdateProduct;

namespace CatalogService.Application.Interfaces
{
    public interface IProductService
    {
        Task<List<GetProductDTO>> GetProductByPageAsync(int take, int page, CancellationToken cancellationToken);
        Task<GetProductDTO> GetProductByIdAsync(int id, CancellationToken cancellationToken);

        Task<bool> CreateProductAsync(CreateProductCommand Product, CancellationToken cancellationToken);
        Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken);
        Task<bool> UpdateProductAsync(UpdateProductCommand Product, CancellationToken cancellationToken);
    }
}
