namespace CatalogService.Domain.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Count { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
