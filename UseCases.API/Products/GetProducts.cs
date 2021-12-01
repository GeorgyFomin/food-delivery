using Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
namespace UseCases.API.Products
{
    public class GetProducts
    {
        public class Query : IRequest<IEnumerable<Product>> { }
        public class QueryHandler : IRequestHandler<Query, IEnumerable<Product>>
        {
            private readonly DataContext _context;
            public QueryHandler(DataContext context) => _context = context;
            public async Task<IEnumerable<Product>> Handle(Query request, CancellationToken cancellationToken) => await _context.Products.ToListAsync(cancellationToken);
        }
    }
}
