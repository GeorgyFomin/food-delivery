using Entities;
using MediatR;
using Persistence.MsSql;
namespace UseCases.API.OrderItems
{
    public class DeleteOrderItem
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
                OrderItem orderItem = await _context.OrderItems.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
                if (orderItem == null) return Unit.Value;
                _context.OrderItems.Remove(orderItem);
                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}
