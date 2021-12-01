using Entities;
using MediatR;
using Persistence.MsSql;

namespace UseCases.API.Ingredients
{
    public class GetIngredientById
    {
        public class Query : IRequest<Ingredient>
        {
            public int Id { get; set; }
        }
        public class QueryHandler : IRequestHandler<Query, Ingredient>
        {
            private readonly DataContext _context;
            public QueryHandler(DataContext context) => _context = context;
            public async Task<Ingredient> Handle(Query request, CancellationToken cancellationToken) =>
                await _context.Ingredients.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
        }
    }
}
