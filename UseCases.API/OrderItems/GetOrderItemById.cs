using Entities;
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
            private readonly DataContext _db;
            public QueryHandler(DataContext db) => _db = db;
            public async Task<OrderItem> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _db.OrderItems.FindAsync(request.Id);
            }
        }
    }
}
