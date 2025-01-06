using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.Common.DTO.BasketDTO;
using OrderService.Application.Common.Response;
using OrderService.Application.Interfaces;
using SharedCollection.Interfaces;

namespace OrderService.Application.Mediator.Queries.BasketQueries
{
    public class GetBasketByOrderIdHandler
        : IRequestHandler<GetBasketByOrderIdQuery, IServiceResponse>
    {
        public GetBasketByOrderIdHandler(
            IOrderServiceContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private readonly IOrderServiceContext _context;
        private readonly IMapper _mapper;

        public async Task<IServiceResponse> Handle(
            GetBasketByOrderIdQuery query,
            CancellationToken token)
        {
            var data = await _context.Baskets
                .AsNoTracking()
                .Where(x => x.OrderId == query.OrderId)
                .ProjectTo<GetBasketDTO>(_mapper.ConfigurationProvider)
                .ToListAsync(token);

            return new ServiceResponseRead<List<GetBasketDTO>>()
            {
                Data = data
            };
        }
    }
}
