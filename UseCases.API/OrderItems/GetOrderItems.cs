using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using UseCases.API.Dto;
using UseCases.API.Exceptions;

namespace UseCases.API.OrderItems
{
    public class GetOrderItems
    {
        public class Query : IRequest<IEnumerable<OrderItemDto>> { }
        public class QueryHandler : IRequestHandler<Query, IEnumerable<OrderItemDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public QueryHandler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<OrderItemDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_context.OrderItems == null)
                {
                    return Enumerable.Empty<OrderItemDto>();
                }
                var orderItems = await _context.OrderItems.ToListAsync(cancellationToken);
                if (orderItems == null)
                {
                    throw new EntityNotFoundException("OrderItems not found");
                }
                return _mapper.Map<List<OrderItemDto>>(orderItems);
            }
        }
    }
}
