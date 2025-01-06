using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.Common.DTO.OrderDTO;
using OrderService.Application.Common.Response;
using OrderService.Application.Interfaces;
using SharedCollection.Interfaces;

namespace OrderService.Application.Mediator.Queries.OrderQueries.GetOrderById
{
    public class GetOrderByIdHandler
        : IRequestHandler<GetOrderByIdQuery, IServiceResponse>
    {

        public GetOrderByIdHandler(
            IOrderServiceContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private readonly IOrderServiceContext _context;
        private readonly IMapper _mapper;

        public async Task<IServiceResponse> Handle(
            GetOrderByIdQuery query,
            CancellationToken token)
        {
            var data = await _context.Orders
                .AsNoTracking()
                .Where(x => x.OrderId == query.OrderId)
                .ProjectTo<GetOrderDTO>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(token);

            return new ServiceResponseRead<GetOrderDTO>()
            {
                Data = data
            };
        }
    }
}
