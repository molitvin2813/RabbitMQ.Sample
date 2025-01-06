using AutoMapper;
using CatalogService.Application.Common.DTO.ProductDTO;
using CatalogService.Application.Interfaces;
using CatalogService.Application.Mediator.Commands.ProductCommands.CreateProduct;
using CatalogService.Application.Mediator.Commands.ProductCommands.UpdateProduct;
using CatalogService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using SharedCollection.Exceptions;

namespace CatalogService.PostgreSQL.Services
{
    public class ProductService : IProductService
    {
        public ProductService(
            IMapper mapper,
            CatalogServiceContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        private readonly CatalogServiceContext _context;
        private readonly IMapper _mapper;

        public async Task<List<GetProductDTO>> GetProductByPageAsync(
            int take,
            int page,
            CancellationToken cancellationToken)
        {
            var data = await _context.Products
                .AsNoTracking()
                .OrderBy(x => x.CreatedDate)
                .Take(take)
                .Skip(page * take)
                .Select(x => new GetProductDTO()
                {
                    ProductId = x.ProductId,
                    Name = x.Name,
                    Price = x.Price,
                    Count = x.Count,
                })
                .ToListAsync(cancellationToken);

            return data;
        }

        public async Task<GetProductDTO> GetProductByIdAsync(int id, CancellationToken cancellationToken)
        {
            var data = await _context.Products
                .AsNoTracking()
                .Select(x => new GetProductDTO()
                {
                    ProductId = x.ProductId,
                    Name = x.Name,
                    Price = x.Price,
                    Count = x.Count,
                })
                .FirstOrDefaultAsync(x => x.ProductId == id, cancellationToken);

            if (data == null)
            {
                throw new RecordNotFoundException<int>(nameof(Product), id);
            }

            return data;
        }

        public async Task<bool> CreateProductAsync(CreateProductCommand product, CancellationToken cancellationToken)
        {
            var data = _mapper.Map<Product>(product);

            data.CreatedDate = DateTime.Now;

            await _context.Products.AddAsync(data, cancellationToken);
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken)
        {
            var data = await _context.Products
              .FirstOrDefaultAsync(x => x.ProductId == id, cancellationToken);

            if (data == null)
            {
                throw new RecordNotFoundException<int>(nameof(Product), id);
            }

            _context.Products.Remove(data);
            return true;
        }

        public async Task<bool> UpdateProductAsync(UpdateProductCommand product, CancellationToken cancellationToken)
        {
            var data = await _context.Products
                .FirstOrDefaultAsync(x => x.ProductId == product.ProductId, cancellationToken);

            if (data == null)
            {
                throw new RecordNotFoundException<int>(nameof(Product), product.ProductId);
            }

            _mapper.Map(product, data);

            return true;
        }
    }
}
