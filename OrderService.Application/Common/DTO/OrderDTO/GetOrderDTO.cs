using OrderService.Application.Common.DTO.BasketDTO;

namespace OrderService.Application.Common.DTO.OrderDTO
{
    public class GetOrderDTO
    {
        public Guid OrderId { get; set; }

        public bool IsConfirmed { get; set; }

        public List<GetBasketDTO> Baskets { get; set; } = new List<GetBasketDTO>();
    }
}
