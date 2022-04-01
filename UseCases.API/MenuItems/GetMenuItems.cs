using AutoMapper;
using Entities.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using UseCases.API.Dto;
using UseCases.API.Exceptions;

namespace UseCases.API.MenuItems
{
    public class GetMenuItems
    {
        public class Query : IRequest<IEnumerable<MenuItemDto>> { }
        public class QueryHandler : IRequestHandler<Query, IEnumerable<MenuItemDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public QueryHandler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<MenuItemDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_context.MenuItems == null)
                {
                    return Enumerable.Empty<MenuItemDto>();
                }
                //var menuItems = await _context.MenuItems.ToListAsync(cancellationToken);
                var menuItems = await _context.MenuItems.Include(e => e.Product).ThenInclude(p => p != null ? p.ProductsIngredients : null).ToListAsync(cancellationToken);
                if (menuItems == null)
                {
                    throw new EntityNotFoundException("MenuItems not found");
                }
                List<MenuItem>? nullItems = new(), allItems = new();
                foreach (var menu in _context.Menus)
                    allItems.AddRange(menu.MenuItems);
                foreach (var menuItem in menuItems)
                {
                    if (allItems.Find(mi => mi.Id == menuItem.Id) == null)
                    {
                        nullItems.Add(menuItem);
                    }
                }
                _context.MenuItems.RemoveRange(nullItems);
                await _context.SaveChangesAsync();
                return _mapper.Map<List<MenuItemDto>>(await _context.MenuItems.ToListAsync(cancellationToken));
            }
        }
    }
}
