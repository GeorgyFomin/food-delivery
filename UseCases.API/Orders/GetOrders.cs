using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using UseCases.API.Dto;
using UseCases.API.Exceptions;

namespace UseCases.API.Orders
{
    public class GetOrders
    {
        public class Query : IRequest<IEnumerable<OrderDto>> { }
        public class QueryHandler : IRequestHandler<Query, IEnumerable<OrderDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public QueryHandler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<IEnumerable<OrderDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_context.Orders == null)
                {
                    return Enumerable.Empty<OrderDto>();
                }
                var orders = await _context.Orders
                    .Include(o => o.Delivery)
                    .Include(o => o.Discount)
                    .Include(o => o.OrderElements)
                    .ThenInclude(oi => oi.Product)
                    //.ThenInclude(p => p.ProductsIngredients)
                    .ToListAsync(cancellationToken);
                if (orders == null)
                {
                    throw new EntityNotFoundException("Order not found");
                }
                foreach (var item in _context.Orders)
                {
                    if (item.OrderElements.Count == 0)
                    {
                        _context.Orders.Remove(item);
                    }
                }
                await _context.SaveChangesAsync();
                return _mapper.Map<List<OrderDto>>(//orders);
                await _context.Orders.ToListAsync());
            }
        }

    }
}