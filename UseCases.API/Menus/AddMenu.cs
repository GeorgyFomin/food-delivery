using Entities.Domain;
using MediatR;
using Persistence.MsSql;

namespace UseCases.API.Menus
{
    public class AddMenu
    {
        public class Command : IRequest<int>
        {
            public ICollection<MenuItem> MenuItems { get; set; } = new HashSet<MenuItem>();
        }
        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly DataContext _context;
            public CommandHandler(DataContext context) => _context = context;
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                Menu menu = new() { MenuItems = request.MenuItems };
                if (_context.Menus == null)
                {
                    return default;
                }
                await _context.Menus.AddAsync(menu, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return menu.Id;
            }
        }
    }
}
