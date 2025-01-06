using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Common.DTO.BasketDTO;
using OrderService.Application.Mediator.Commands.BasketCommands.CreateBasket;
using OrderService.Application.Mediator.Queries.BasketQueries;
using SharedCollection.Interfaces;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class BasketController : ControllerBase
    {
        public BasketController(
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IServiceResponse>> GetById(Guid orderId)
        {
            return Ok(await _mediator.Send(new GetBasketByOrderIdQuery()
            {
                OrderId = orderId
            }));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IServiceResponse>> CreateOrder([FromBody] AddBasketDTO dto)
        {
            var command = _mapper.Map<CreateBasketCommand>(dto);

            return Ok(await _mediator.Send(command));
        }
    }
}
