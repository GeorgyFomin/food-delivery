using Entities.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
namespace UseCases.API.OrderItems
{
    public class GetOrderItems
    {
        public class Query : IRequest<IEnumerable<OrderItem>> { }
        public class QueryHandler : IRequestHandler<Query, IEnumerable<OrderItem>>
        {
            private readonly DataContext _context;
            public QueryHandler(DataContext context) => _context = context;
            public async Task<IEnumerable<OrderItem>> Handle(Query request, CancellationToken cancellationToken) =>
                await _context.OrderItems.ToListAsync(cancellationToken);
        }
    }
}
