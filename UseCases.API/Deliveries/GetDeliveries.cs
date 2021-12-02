using Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;

namespace UseCases.API.Deliveries
{
    public class GetDeliveries
    {
        public class Query : IRequest<IEnumerable<Delivery>> { }
        public class QueryHandler : IRequestHandler<Query, IEnumerable<Delivery>>
        {
            private readonly DataContext _context;
            public QueryHandler(DataContext context) => _context = context;
            public async Task<IEnumerable<Delivery>> Handle(Query request, CancellationToken cancellationToken) =>
                await _context.Deliveries.ToListAsync(cancellationToken);
        }
    }
}
