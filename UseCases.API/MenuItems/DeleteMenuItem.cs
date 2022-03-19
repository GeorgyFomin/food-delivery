using Entities.Domain;
using MediatR;
using Persistence.MsSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseCases.API.MenuItems
{
    public class DeleteMenuItem
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
                if (_context.MenuItems == null)
                {
                    return Unit.Value;
                }
                MenuItem? menuItem = await _context.MenuItems.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
                if (menuItem == null) return Unit.Value;
                _context.MenuItems.Remove(menuItem);
                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}
