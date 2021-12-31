using Entities;
using Entities.Domain;
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
            private readonly DataContext _context;
            public QueryHandler(DataContext context) => _context = context;
            public async Task<Product> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Products.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
            }
        }
    }
}
