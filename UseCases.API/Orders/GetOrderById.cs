using Entities;
using MediatR;
using Persistence.MsSql;
namespace UseCases.API.Orders
{
    public class GetOrderById
    {
        public class Query : IRequest<Order>
        {
            public int Id { get; set; }
        }
        public class QueryHandler : IRequestHandler<Query, Order>
        {
            private readonly DataContext _context;
            public QueryHandler(DataContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));
            public async Task<Order> Handle(Query request, CancellationToken cancellationToken) => 
                await _context.Orders.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
        }
    }
}
