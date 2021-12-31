using Entities;
using Entities.Domain;
using MediatR;
using Persistence.MsSql;
using UseCases.API.Deliveries.Dto;

namespace UseCases.API.Products
{
    public class GetProductById
    {
        public class Query : IRequest<ProductDto>
        {
            public int Id { get; set; }
        }
        public class QueryHandler : IRequestHandler<Query, ProductDto>
        {
            private readonly DataContext _context;
            public QueryHandler(DataContext context) => _context = context;
            public async Task<ProductDto> Handle(Query request, CancellationToken cancellationToken)
            {
                Product? product = await _context.Products.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
                return new ProductDto { Id = product.Id, Price = product.Price, Name = product.Name, Weight = product.Weight, Ingredients = product.Ingredients };
            }
        }
    }
}
