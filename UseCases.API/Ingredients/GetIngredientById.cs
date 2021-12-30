using Entities;
using Entities.Domain;
using MediatR;
using Persistence.MsSql;
using UseCases.API.Deliveries.Dto;

namespace UseCases.API.Ingredients
{
    public class GetIngredientById
    {
        public class Query : IRequest<IngredientDto>
        {
            public int Id { get; set; }
        }
        public class QueryHandler : IRequestHandler<Query, IngredientDto>
        {
            private readonly DataContext _context;
            public QueryHandler(DataContext context) => _context = context;
            public async Task<IngredientDto> Handle(Query request, CancellationToken cancellationToken)
            {
                Ingredient ingredient= await _context.Ingredients.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
                return new IngredientDto() { Name = ingredient.Name, Id = ingredient.Id };
            }
        }
    }
}
