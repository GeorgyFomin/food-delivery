using Entities.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;

namespace UseCases.API.Menus
{
    public class EditMenu
    {
        public class Command : IRequest<int>
        {
            public int Id { get; set; }
            public List<MenuItem> MenuItems { get; set; } = new();
        }
        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly DataContext _context;

            public CommandHandler(DataContext context) => _context = context;
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                if (_context.Menus == null)
                {
                    return default;
                }
                Menu? menu = await _context.Menus.Include(e => e.MenuItems).FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken: cancellationToken);
                if (menu == null)
                {
                    return default;
                }
                menu.MenuItems = request.MenuItems;
                await _context.SaveChangesAsync(cancellationToken);
                return menu.Id;
            }
        }
    }
}
