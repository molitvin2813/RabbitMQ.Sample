using AutoMapper;
using OrderService.Application.Common.DTO.OrderDTO;
using OrderService.Domain.Models;

namespace OrderService.Application.Common.AutoMapper
{
    public class OrderProfile
        : Profile
    {
        public OrderProfile()
        {
            CreateProjection<Order, GetOrderDTO>();
        }
    }
}
