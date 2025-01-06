using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Mediator.Commands.OrderCommands.ConfirmOrder;
using OrderService.Application.Mediator.Commands.OrderCommands.CreateOrder;
using OrderService.Application.Mediator.Queries.OrderQueries.GetOrderById;
using OrderService.Application.Mediator.Queries.OrderQueries.GetOrderByPage;
using SharedCollection.Interfaces;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class OrderController : ControllerBase
    {
        public OrderController(
            IMediator mediator,
            IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("{page}/{take}")]
        public async Task<ActionResult<IServiceResponse>> GetConfirmedOrder([FromRoute] int page, [FromRoute] int take)
        {
            return Ok(await _mediator.Send(new GetOrderByPageQuery()
            {
                Page = page,
                Take = take,
                IsConfirmed = true
            }));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("{page}/{take}")]
        public async Task<ActionResult<IServiceResponse>> GetUnconfirmedOrder([FromRoute] int page, [FromRoute] int take)
        {
            return Ok(await _mediator.Send(new GetOrderByPageQuery()
            {
                Page = page,
                Take = take,
                IsConfirmed = false
            }));
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IServiceResponse>> GetById(Guid orderId)
        {
            return Ok(await _mediator.Send(new GetOrderByIdQuery()
            {
                OrderId = orderId
            }));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IServiceResponse>> CreateOrder()
        {
            return Ok(await _mediator.Send(new CreateOrderCommand()));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IServiceResponse>> ConfirmOrder(Guid orderId)
        {

            return Ok(await _mediator.Send(new ConfirmOrderCommand()
            {
                OrderId = orderId
            }));
        }
    }
}
