using Entities.Domain;
using MediatR;
using Persistence.MsSql;

namespace UseCases.API.MenuItems
{
    public class EditMenuItem
    {
        public class Command : IRequest<int>
        {
            public int Id { get; set; }
            public Product? Product { get; set; }
        }
        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly DataContext _context;

            public CommandHandler(DataContext context) => _context = context;
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                if (_context.MenuItems == null)
                {
                    return default;
                }
                MenuItem? menuItem = await _context.MenuItems.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
                if (menuItem == null)
                {
                    return default;
                }
                menuItem.Product = request.Product;
                await _context.SaveChangesAsync(cancellationToken);
                return menuItem.Id;
            }
        }
    }
}
