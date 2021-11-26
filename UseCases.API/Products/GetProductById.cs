using Entities;
using MediatR;
using Persistence.MsSql;
namespace UseCases.API.Products
{
    public class GetProductById
    {
        public class Query : IRequest<Product>
        {
            public int Id { get; set; }
        }
        public class QueryHandler : IRequestHandler<Query, Product>
        {
            private readonly DataContext _db;
            public QueryHandler(DataContext db) => _db = db;
            public async Task<Product> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _db.Products.FindAsync(request.Id);
            }
        }
    }
}
