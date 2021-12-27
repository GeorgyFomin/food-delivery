using Entities.Domain;
using MediatR;
using Persistence.MsSql;
namespace UseCases.API.OrderItems
{
    public class GetOrderItemById
    {
        public class Query : IRequest<OrderItem>
        {
            public int Id { get; set; }
        }
        public class QueryHandler : IRequestHandler<Query, OrderItem>
        {
            private readonly DataContext _context;
            public QueryHandler(DataContext context) => _context = context;
            public async Task<OrderItem> Handle(Query request, CancellationToken cancellationToken) =>
                await _context.OrderItems.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
        }
    }
}
