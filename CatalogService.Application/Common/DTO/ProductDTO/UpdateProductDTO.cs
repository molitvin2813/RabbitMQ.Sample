namespace CatalogService.Application.Common.DTO.ProductDTO
{
    public class UpdateProductDTO
    {
        public required int ProductId { get; set; }

        public string? Name { get; set; }

        public decimal? Price { get; set; }

        public int? Count { get; set; }
    }
}
