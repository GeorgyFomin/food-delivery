using Entities;
using Entities.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using UseCases.API.Deliveries.Dto;

namespace UseCases.API.Deliveries
{
    public class GetDeliveries
    {
        public class Query : IRequest<IEnumerable<Delivery>> { }
        public class QueryHandler : IRequestHandler<Query, IEnumerable<Delivery>>
        {
            private readonly DataContext _context;
            public QueryHandler(DataContext context) => _context = context;
            public async Task<IEnumerable<Delivery>> Handle(Query request, CancellationToken cancellationToken)
            {
              return await _context.Deliveries.ToListAsync(cancellationToken);
            }
        }
    }
}
