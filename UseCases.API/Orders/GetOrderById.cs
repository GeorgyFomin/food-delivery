using AutoMapper;
using MediatR;
using Persistence.MsSql;
using UseCases.API.Dto;
using UseCases.API.Exceptions;

namespace UseCases.API.Orders
{
    public class GetOrderById
    {
        public class Query : IRequest<OrderDto?>
        {
            public int Id { get; set; }
        }
        public class QueryHandler : IRequestHandler<Query, OrderDto?>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public QueryHandler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<OrderDto?> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_context.Orders == null)
                {
                    return null;
                }
                var order = await _context.Orders.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
                if (order == null)
                {
                    throw new EntityNotFoundException("Order not found");
                }
                return _mapper.Map<OrderDto?>(order);
            }
        }
    }
}
