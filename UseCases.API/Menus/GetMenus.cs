using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using UseCases.API.Dto;
using UseCases.API.Exceptions;

namespace UseCases.API.Menus
{
    public class GetMenus
    {
        public class Query : IRequest<IEnumerable<MenuDto>> { }
        public class QueryHandler : IRequestHandler<Query, IEnumerable<MenuDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public QueryHandler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<IEnumerable<MenuDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_context.Menus == null)
                {
                    return Enumerable.Empty<MenuDto>();
                }
                var menus = await _context
                    .Menus
                    .Include(m => m.MenuItems)
                    .ThenInclude(mi => mi.Product)
                    .ThenInclude(p => p == null ? null : p.ProductsIngredients)
                    .ToListAsync(cancellationToken);
                if (menus == null)
                {
                    throw new EntityNotFoundException("Menu not found");
                }
                foreach (var item in _context.Menus)
                {
                    if (item.MenuItems.Count == 0)
                    {
                        _context.Menus.Remove(item);
                    }
                }
                await _context.SaveChangesAsync();
                return _mapper.Map<List<MenuDto>>(await _context.Menus.ToListAsync());
            }
        }
    }
}
