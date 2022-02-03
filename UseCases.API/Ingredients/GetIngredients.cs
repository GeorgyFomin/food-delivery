using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using UseCases.API.Dto;
using UseCases.API.Exceptions;

namespace UseCases.API.Ingredients
{
    public class GetIngredients
    {
        public class Query : IRequest<IEnumerable<IngredientDto>> { }
        public class QueryHandler : IRequestHandler<Query, IEnumerable<IngredientDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public QueryHandler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<IngredientDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var ingredients = await _context.Ingredients.ToListAsync(cancellationToken);//.Include(e => e.Products)
                if (ingredients == null)
                {
                    throw new EntityNotFoundException("Ingredients not found");
                }
                List<IngredientDto> ingredientDtos = new();
                _mapper.Map(ingredients, ingredientDtos);
                return ingredientDtos;
            }
        }
    }
}
