using Entities;
using Entities.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using UseCases.API.Deliveries.Dto;

namespace UseCases.API.Deliveries
{
    public class GetDeliveries
    {
        public class Query : IRequest<IEnumerable<DeliveryDto>> { }
        public class QueryHandler : IRequestHandler<Query, IEnumerable<DeliveryDto>>
        {
            private readonly DataContext _context;
            public QueryHandler(DataContext context) => _context = context;
            public async Task<IEnumerable<DeliveryDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var deliveries = await _context.Deliveries.ToListAsync(cancellationToken);
                return from d in deliveries
                       select new DeliveryDto()
                       {
                           Id = d.Id,
                           Price = d.Price,
                           ServiceName = d.ServiceName,
                           TimeSpan = d.TimeSpan
                       };
            }
        }
    }
}
