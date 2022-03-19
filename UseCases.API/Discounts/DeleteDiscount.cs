using MediatR;
using Persistence.MsSql;

namespace UseCases.API.Discounts
{
    public class DeleteDiscount
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
                if (_context.Discounts == null) return Unit.Value;
                var discount = await _context.Discounts.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
                if (discount == null) return Unit.Value;
                _context.Discounts.Remove(discount);
                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}
