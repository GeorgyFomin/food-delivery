using Entities;
using Entities.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using UseCases.API.Deliveries.Dto;

namespace UseCases.API.Ingredients
{
    public class GetIngredients
    {
        public class Query : IRequest<IEnumerable<IngredientDto>> { }
        public class QueryHandler : IRequestHandler<Query, IEnumerable<IngredientDto>>
        {
            private readonly DataContext _context;
            public QueryHandler(DataContext context) => _context = context;
            public async Task<IEnumerable<IngredientDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var ingredients = await _context.Ingredients.ToListAsync(cancellationToken);
                return from ingr in ingredients
                       select new IngredientDto()
                       {
                           Id = ingr.Id,
                           Name = ingr.Name
                       };
            }
        }
    }
}
