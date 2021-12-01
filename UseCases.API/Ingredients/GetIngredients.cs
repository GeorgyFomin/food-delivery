using Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;

namespace UseCases.API.Ingredients
{
    public class GetIngredients
    {
        public class Query : IRequest<IEnumerable<Ingredient>> { }
        public class QueryHandler : IRequestHandler<Query, IEnumerable<Ingredient>>
        {
            private readonly DataContext _context;
            public QueryHandler(DataContext context) => _context = context;
            public async Task<IEnumerable<Ingredient>> Handle(Query request, CancellationToken cancellationToken) => 
                await _context.Ingredients.ToListAsync(cancellationToken);
        }
    }
}
