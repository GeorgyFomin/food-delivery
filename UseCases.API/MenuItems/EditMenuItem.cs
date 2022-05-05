using Entities.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using UseCases.API.Dto;

namespace UseCases.API.MenuItems
{
    public class EditMenuItem
    {
        public class Command : IRequest<int>
        {
            public int Id { get; set; }
            public ProductDto? Product { get; set; }
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
                if (menuItem == null || request.Product == null)
                {
                    return default;
                }
                Product? product = await _context.Products.Include(e => e.ProductsIngredients).
                    FirstOrDefaultAsync(p => p.Id == request.Product.Id, cancellationToken: cancellationToken);
                if (product == null)
                {
                    return default;
                }
                menuItem.Product = product;
                await _context.SaveChangesAsync(cancellationToken);
                return menuItem.Id;
            }
        }
    }
}
