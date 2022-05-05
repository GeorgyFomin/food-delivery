using Entities.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using UseCases.API.Dto;

namespace UseCases.API.Menus
{
    public class EditMenu
    {
        public class Command : IRequest<int>
        {
            public int Id { get; set; }
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
                Menu? menu = await _context.Menus.Include(e => e.MenuItems).FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken: cancellationToken);
                if (menu == null)
                {
                    return default;
                }
                List<MenuItem> menuItems = menu.MenuItems.ToList();
                MenuItemDto? item = request.MenuItems.ToList().Find(oi => oi.Id == 0);
                if (item != null && item.Product != null)
                {
                    Product? product = await _context.Products.Include(e => e.ProductsIngredients).
                        FirstOrDefaultAsync(p => p.Id == item.Product.Id, cancellationToken: cancellationToken);
                    MenuItem menuItem = new() { Product = product };
                    if (_context.OrderItems == null)
                    {
                        return default;
                    }
                    await _context.MenuItems.AddAsync(menuItem, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                    menuItems.Add(menuItem);
                }
                menu.MenuItems = menuItems;
                await _context.SaveChangesAsync(cancellationToken);
                return menu.Id;
            }
        }
    }
}
