using AutoMapper;
using MediatR;
using Persistence.MsSql;
using UseCases.API.Dto;
using UseCases.API.Exceptions;

namespace UseCases.API.Discounts
{
    public class GetDiscountById
    {
        public class Query : IRequest<DiscountDto?>
        {
            public int Id { get; set; }
        }
        public class QueryHandler : IRequestHandler<Query, DiscountDto?>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public QueryHandler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<DiscountDto?> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_context.Discounts == null)
                {
                    return null;
                }
                var discount = await _context.Discounts.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
                if (discount == null)
                {
                    throw new EntityNotFoundException("Discount not found");
                }
                return _mapper.Map<DiscountDto?>(discount);
            }
        }
    }
}
