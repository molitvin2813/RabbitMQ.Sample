namespace OrderService.Domain.Models
{
    public class Basket
    {
        public int BasketId { get; set; }
        public Guid OrderId { get; set; }

        public int ProductId { get; set; }

        public int Count { get; set; }

        //public string Name { get; set; } = null!;

        //public decimal Price { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public Order Order { get; set; } = null!;
    }
}
