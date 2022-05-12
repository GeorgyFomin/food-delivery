using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseCases.API.Dto;
using UseCases.API.Exceptions;

namespace UseCases.API.MenuItems
{
    public class GetMenuItemById
    {
        public class Query : IRequest<MenuItemDto?>
        {
            public int Id { get; set; }
        }
        public class QueryHandler : IRequestHandler<Query, MenuItemDto?>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public QueryHandler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<MenuItemDto?> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_context.MenuItems == null)
                {
                    return null;
                }
                var menuItem = await _context
                    .MenuItems
                    .Include(e => e.Product)
                    .ThenInclude(p => p != null ? p.ProductsIngredients : null)
                    .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
                if (menuItem == null)
                {
                    throw new EntityNotFoundException("MenuItem not found");
                }
                return _mapper.Map<MenuItemDto?>(menuItem);
            }
        }
    }
}
