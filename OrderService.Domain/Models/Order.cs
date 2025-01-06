namespace OrderService.Domain.Models
{
    public class Order
    {
        public Guid OrderId { get; set; }

        public bool IsConfirmed { get; set; }

        public DateTime CrateDate { get; set; }

        public DateTime? ConfirmedDate { get; set; }

        public ICollection<Basket> Baskets { get; set; } = new List<Basket>();
    }
}
