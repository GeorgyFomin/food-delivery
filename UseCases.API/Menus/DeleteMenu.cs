using Entities.Domain;
using MediatR;
using Persistence.MsSql;

namespace UseCases.API.Menus
{
    public class DeleteMenu
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
                if (_context.Menus == null)
                {
                    return Unit.Value;
                }
                Menu? menu = await _context.Menus.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
                if (menu == null) return Unit.Value;
                _context.Menus.Remove(menu);
                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}
