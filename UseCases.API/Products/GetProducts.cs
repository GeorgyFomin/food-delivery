using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using UseCases.API.Dto;

namespace UseCases.API.Products
{
    public class GetProducts
    {
        public class Query : IRequest<IEnumerable<ProductDto>> { }
        public class QueryHandler : IRequestHandler<Query, IEnumerable<ProductDto>>
        {
            private readonly DataContext _context;
            public QueryHandler(DataContext context) => _context = context;
            public async Task<IEnumerable<ProductDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var products = await _context.Products.Include(e => e.Ingredients).ToListAsync(cancellationToken);
                return from p in products
                       select new ProductDto()
                       {
                           Id = p.Id,
                           Name = p.Name,
                           Price = p.Price,
                           Weight = p.Weight
                       };
            }
        }
    }
}
