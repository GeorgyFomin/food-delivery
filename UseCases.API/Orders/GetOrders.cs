using Entities.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;

namespace UseCases.API.Orders
{
    public class GetOrders
    {
        public class Query : IRequest<IEnumerable<Order>> { }
        public class QueryHandler : IRequestHandler<Query, IEnumerable<Order>>
        {
            private readonly DataContext _context;
            public QueryHandler(DataContext context) => _context = context;
            public async Task<IEnumerable<Order>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Orders.ToListAsync(cancellationToken);
            }
        }

    }
}