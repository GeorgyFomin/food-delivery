using AutoMapper;
using Entities.Domain;
using MediatR;
using Persistence.MsSql;
using UseCases.API.Dto;
using UseCases.API.Exceptions;

namespace UseCases.API.Ingredients
{
    public class GetIngredientById
    {
        public class Query : IRequest<IngredientDto?>
        {
            public int Id { get; set; }
        }
        public class QueryHandler : IRequestHandler<Query, IngredientDto?>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public QueryHandler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IngredientDto?> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_context.Ingredients == null)
                {
                    return null;
                }
                Ingredient? ingredient = await _context.Ingredients.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
                if (ingredient == null)
                {
                    throw new EntityNotFoundException("Ingredient not found");
                }
                return _mapper.Map<IngredientDto?>(ingredient);
            }
        }
    }
}
