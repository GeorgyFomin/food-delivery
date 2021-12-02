using Entities;
using MediatR;
using Persistence.MsSql;

namespace UseCases.API.Deliveries
{
    public class GetDeliveryById
    {
        public class Query : IRequest<Delivery>
        {
            public int Id { get; set; }
        }
        public class QueryHandler : IRequestHandler<Query, Delivery>
        {
            private readonly DataContext _context;
            public QueryHandler(DataContext context) => _context = context;
            public async Task<Delivery> Handle(Query request, CancellationToken cancellationToken) =>
                await _context.Deliveries.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
        }
    }
}
