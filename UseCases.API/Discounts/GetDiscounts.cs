using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using UseCases.API.Dto;
using UseCases.API.Exceptions;

namespace UseCases.API.Discounts
{
    public class GetDiscounts
    {
        public class Query : IRequest<IEnumerable<DiscountDto>> { }
        public class QueryHandler : IRequestHandler<Query, IEnumerable<DiscountDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public QueryHandler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<DiscountDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_context.Discounts == null)
                {
                    return Enumerable.Empty<DiscountDto>();
                }
                var discounts = await _context.Discounts.ToListAsync(cancellationToken);
                if (discounts == null)
                {
                    throw new EntityNotFoundException("Discounts not found");
                }
                return _mapper.Map<List<DiscountDto>>(discounts);
            }
        }
    }
}
