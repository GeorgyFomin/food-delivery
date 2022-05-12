using AutoMapper;
using Entities.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using UseCases.API.Dto;
using UseCases.API.Exceptions;

namespace UseCases.API.Products
{
    public class GetProductById
    {
        public class Query : IRequest<ProductDto?>
        {
            public int Id { get; set; }
        }
        public class QueryHandler : IRequestHandler<Query, ProductDto?>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public QueryHandler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<ProductDto?> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_context.Products == null)
                {
                    return null;
                }
                Product? product = await _context
                    .Products
                    .Include(e => e.ProductsIngredients)
                    .ThenInclude(p => p.Ingredient)
                    .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
                //.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
                if (product == null)
                {
                    throw new EntityNotFoundException("Product not found");
                }
                return _mapper.Map<ProductDto?>(product);
            }
        }
    }
}
