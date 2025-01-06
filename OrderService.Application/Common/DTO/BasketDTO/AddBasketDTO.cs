namespace OrderService.Application.Common.DTO.BasketDTO
{
    public class AddBasketDTO
    {
        public Guid OrderId { get; set; }

        public int ProductId { get; set; }

        public int Count { get; set; }
    }
}
