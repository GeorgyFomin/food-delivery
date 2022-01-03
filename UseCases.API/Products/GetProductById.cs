using AutoMapper;
using Entities.Domain;
using MediatR;
using Persistence.MsSql;
using UseCases.API.Dto;
using UseCases.API.Exceptions;

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
            private readonly IMapper _mapper;

            public QueryHandler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<ProductDto> Handle(Query request, CancellationToken cancellationToken)
            {
                Product? product = await _context.Products.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
                if (product == null)
                {
                    throw new EntityNotFoundException("Product not found");
                }
                ProductDto productDto = new();
                _mapper.Map(product, productDto);
                return productDto;
            }
        }
    }
}
