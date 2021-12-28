using Entities;
using MediatR;
using Persistence.MsSql;
using Entities.Domain;

namespace UseCases.API.Deliveries
{
    public class AddDelivery
    {
        public class Command : IRequest<int>
        {
            public string ServiceName { get; set; }
            public decimal Price { get; set; }
            public TimeSpan TimeSpan { get; set; }
        }
        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly DataContext _context;

            public CommandHandler(DataContext context) => _context = context;
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                Delivery Delivery = new() { ServiceName = request.ServiceName, Price = request.Price, TimeSpan = request.TimeSpan };
                await _context.Deliveries.AddAsync(Delivery, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Delivery.Id;
            }
        }
    }
}
