using MediatR;
using OrderService.Application.Common.Response;
using OrderService.Application.Interfaces;
using SharedCollection.Interfaces;

namespace OrderService.Application.Mediator.Commands.BasketCommands.UpdateBasket
{
    public class UpdateBasketHandler
        : IRequestHandler<UpdateBasketCommand, IServiceResponse>
    {
        public UpdateBasketHandler(IOrderServiceContext context)
        {
            _context = context;
        }

        private readonly IOrderServiceContext _context;

        public async Task<IServiceResponse> Handle(
            UpdateBasketCommand command,
            CancellationToken token)
        {

            //await _context.Baskets
            //    .FirstOrDefaultAsync(x=> x.BasketId)

            return new ServiceResponseWrite<int>();
        }
    }
}
