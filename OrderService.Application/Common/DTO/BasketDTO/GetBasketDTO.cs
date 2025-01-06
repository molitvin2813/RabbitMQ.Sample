namespace OrderService.Application.Common.DTO.BasketDTO
{
    public class GetBasketDTO
    {
        public int BasketId { get; set; }

        public Guid OrderId { get; set; }

        public int ProductId { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }
    }
}
