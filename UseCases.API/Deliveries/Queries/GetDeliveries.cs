using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using UseCases.API.Dto;
using UseCases.API.Exceptions;

namespace UseCases.API.Deliveries
{
    public class GetDeliveries
    {
        public class Query : IRequest<IEnumerable<DeliveryDto>> { }
        public class QueryHandler : IRequestHandler<Query, IEnumerable<DeliveryDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public QueryHandler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<DeliveryDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_context.Deliveries == null)
                {
                    return Enumerable.Empty<DeliveryDto>();
                }
                var deliveries = await _context.Deliveries.ToListAsync(cancellationToken);
                if (deliveries == null)
                {
                    throw new EntityNotFoundException("Deliveries not found");
                }
                return _mapper.Map<List<DeliveryDto>>(deliveries);
            }
        }
    }
}
