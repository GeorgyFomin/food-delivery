using Entities;
using MediatR;
using Persistence.MsSql;
using Entities.Domain;

namespace UseCases.API.Deliveries
{
    public class DeleteDelivery
    {
        public class Command : IRequest
        {
            public int Id { get; set; }
        }
        public class CommandHandler : IRequestHandler<Command, Unit>
        {
            private readonly DataContext _context;
            public CommandHandler(DataContext context) => _context = context;
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                if (_context.Deliveries == null) return Unit.Value;
                Delivery? Delivery = await _context.Deliveries.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
                if (Delivery == null) return Unit.Value;
                _context.Deliveries.Remove(Delivery);
                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}
