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

namespace UseCases.API.Menus
{
    public class GetMenuById
    {
        public class Query : IRequest<MenuDto?>
        {
            public int Id { get; set; }
        }
        public class QueryHandler : IRequestHandler<Query, MenuDto?>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public QueryHandler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<MenuDto?> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_context.Menus == null)
                {
                    return null;
                }
                var menu = await _context
                    .Menus
                    .Include(m => m.MenuItems)
                    .ThenInclude(mi => mi.Product)
                    .ThenInclude(p => p == null ? null : p.ProductsIngredients)
                    .FirstOrDefaultAsync(mi => mi.Id == request.Id, cancellationToken);
                if (menu == null)
                {
                    throw new EntityNotFoundException("Menu not found");
                }
                return _mapper.Map<MenuDto?>(menu);
            }
        }
    }
}
