using Entities.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using UseCases.API.Dto;

namespace UseCases.API.Menus
{
    public class AddMenu
    {
        public class Command : IRequest<int>
        {
            public List<MenuItemDto> MenuItems { get; set; } = new();
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
                List<MenuItem> menuItems = new();
                List<MenuItemDto>? menuItemDtos = request.MenuItems.ToList();
                foreach (MenuItemDto item in menuItemDtos)
                {
                    if (item.Product != null)
                    {
                        Product? product = await _context.Products.Include(e => e.ProductsIngredients).
                            FirstOrDefaultAsync(p => p.Id == item.Product.Id, cancellationToken: cancellationToken);
                        MenuItem menuItem = new() { Product = product };
                        await _context.MenuItems.AddAsync(menuItem, cancellationToken);
                        await _context.SaveChangesAsync(cancellationToken);
                        menuItems.Add(menuItem);
                    }
                }

                Menu menu = new() { MenuItems = menuItems };
                await _context.Menus.AddAsync(menu, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return menu.Id;
            }
        }
    }
}
