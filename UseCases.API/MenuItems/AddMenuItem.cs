using Entities.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseCases.API.Dto;

namespace UseCases.API.MenuItems
{
    public class AddMenuItem
    {
        public class Command : IRequest<int>
        {
            public ProductDto? Product { get; set; }
        }
        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly DataContext _context;
            public CommandHandler(DataContext context) => _context = context;
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                if (_context.MenuItems == null || request.Product == null)
                {
                    return default;
                }
                Product? product = await _context
                    .Products
                    .Include(e => e.ProductsIngredients)
                    .FirstOrDefaultAsync(p => p.Id == request.Product.Id, cancellationToken: cancellationToken);
                if (product == null)
                {
                    return default;
                }
                MenuItem menuItem = new() { Product = product };
                await _context.MenuItems.AddAsync(menuItem, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return menuItem.Id;
            }
        }
    }
}
