using Entities;
using Entities.Domain;
using MediatR;
using Persistence.MsSql;
using UseCases.API.Deliveries.Dto;

namespace UseCases.API.Deliveries
{
    public class GetDeliveryById
    {
        public class Query : IRequest<DeliveryDto>
        {
            public int Id { get; set; }
        }
        public class QueryHandler : IRequestHandler<Query, DeliveryDto>
        {
            private readonly DataContext _context;
            public QueryHandler(DataContext context) => _context = context;
            public async Task<DeliveryDto> Handle(Query request, CancellationToken cancellationToken)
            {
                Delivery? delivery = await _context.Deliveries.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
                return new DeliveryDto()
                {
                    TimeSpan = delivery.TimeSpan,
                    Price = delivery.Price,
                    ServiceName = delivery.ServiceName,
                    Id = delivery.Id
                };
            }
        }
    }
}
