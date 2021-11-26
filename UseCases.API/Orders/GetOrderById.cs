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
            private readonly DataContext _db;
            public QueryHandler(DataContext db) => _db = db;
            public async Task<Order> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _db.Orders.FindAsync(request.Id);
            }
        }
    }
}
