using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.Common.DTO.OrderDTO;
using OrderService.Application.Common.Response;
using OrderService.Application.Interfaces;
using SharedCollection.Interfaces;

namespace OrderService.Application.Mediator.Queries.OrderQueries.GetOrderByPage
{
    public class GetOrderByPageHandler
        : IRequestHandler<GetOrderByPageQuery, IServiceResponse>
    {
        public GetOrderByPageHandler(
            IOrderServiceContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private readonly IOrderServiceContext _context;
        private readonly IMapper _mapper;
        public async Task<IServiceResponse> Handle(
            GetOrderByPageQuery query,
            CancellationToken token)
        {
            var data = await _context.Orders
                .AsNoTracking()
                .Where(x => x.IsConfirmed == query.IsConfirmed)
                .OrderByDescending(x => x.CrateDate)
                .Skip(query.Take * query.Page)
                .Take(query.Take)
                .ProjectTo<GetOrderDTO>(_mapper.ConfigurationProvider)
                .ToListAsync(token);

            return new ServiceResponseRead<List<GetOrderDTO>>()
            {
                Data = data
            };
        }
    }
}
