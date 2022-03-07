using Entities.Domain;
using MediatR;
using Persistence.MsSql;
namespace UseCases.API.Products
{
    public class DeleteProduct
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
                if (_context.Products==null)
                {
                    return Unit.Value;
                }
                Product? product = await _context.Products.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
                if (product == null) return Unit.Value;
                _context.Products.Remove(product);
                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}
