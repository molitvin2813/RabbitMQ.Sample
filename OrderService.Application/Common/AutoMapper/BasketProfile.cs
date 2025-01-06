using AutoMapper;
using OrderService.Application.Common.DTO.BasketDTO;
using OrderService.Application.Mediator.Commands.BasketCommands.CreateBasket;
using OrderService.Domain.Models;

namespace OrderService.Application.Common.AutoMapper
{
    internal class BasketProfile : Profile
    {
        public BasketProfile()
        {
            CreateMap<AddBasketDTO, CreateBasketCommand>();

            CreateProjection<Basket, GetBasketDTO>();
        }
    }
}
