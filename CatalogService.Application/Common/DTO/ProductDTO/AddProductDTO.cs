namespace CatalogService.Application.Common.DTO.ProductDTO
{
    public class AddProductDTO
    {
        public required string Name { get; set; }

        public decimal Price { get; set; }

        public int Count { get; set; }
    }
}
