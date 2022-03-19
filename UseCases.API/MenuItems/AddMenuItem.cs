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
    public class AddMenuItem
    {
        public class Command : IRequest<int>
        {
            public Product? Product { get; set; }
        }
        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly DataContext _context;
            public CommandHandler(DataContext context) => _context = context;
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                MenuItem menuItem = new() { Product = request.Product };
                if (_context.MenuItems == null)
                {
                    return default;
                }
                await _context.MenuItems.AddAsync(menuItem, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return menuItem.Id;
            }
        }
    }
}
