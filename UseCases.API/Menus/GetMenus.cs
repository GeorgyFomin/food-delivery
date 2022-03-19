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
                var menu = await _context.Menus.ToListAsync(cancellationToken);
                if (menu == null)
                {
                    throw new EntityNotFoundException("Menu not found");
                }
                return _mapper.Map<List<MenuDto>>(menu);
            }
        }
    }
}
