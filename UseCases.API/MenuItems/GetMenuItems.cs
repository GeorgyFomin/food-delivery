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
                var menuItems = await _context.MenuItems.ToListAsync(cancellationToken);
                if (menuItems == null)
                {
                    throw new EntityNotFoundException("MenuItems not found");
                }
                return _mapper.Map<List<MenuItemDto>>(menuItems);
            }
        }
    }
}
