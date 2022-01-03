using AutoMapper;
using Entities.Domain;
using MediatR;
using Persistence.MsSql;
using UseCases.API.Dto;
using UseCases.API.Exceptions;

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
            private readonly IMapper _mapper;
            public QueryHandler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<DeliveryDto> Handle(Query request, CancellationToken cancellationToken)
            {
                Delivery? delivery = await _context.Deliveries.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
                if (delivery == null)
                {
                    throw new EntityNotFoundException("Delivery not found");
                }
                DeliveryDto deliveryDto = new();
                _mapper.Map(delivery, deliveryDto);
                return deliveryDto;
            }
        }
    }
}
