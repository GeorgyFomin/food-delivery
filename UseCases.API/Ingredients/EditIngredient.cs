using Entities.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using UseCases.API.Dto;

namespace UseCases.API.Ingredients
{
    public class EditIngredient
    {
        static List<ProductIngredient> GetProductIngredients(List<ProductIngredientDto>? productIngredientDtos)
        {
            List<ProductIngredient> productIngredients = new();
            if (productIngredientDtos != null)
            {
                foreach (ProductIngredientDto productIngredientDto in productIngredientDtos)
                {
                    productIngredients.Add(new ProductIngredient() { IngredientId = productIngredientDto.IngredientId, ProductId = productIngredientDto.ProductId });
                }
            }
            return productIngredients;
        }
        public class Command : IRequest<int>
        {
            public int Id { get; set; }
            public string Name { get; set; } = "Noname";
            public List<ProductIngredientDto>? ProductsIngredients { get; set; }
        }
        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly DataContext _context;

            public CommandHandler(DataContext context) => _context = context;
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                if (_context.Ingredients == null)
                    return default;
                Ingredient? ingredient = await _context.Ingredients.Include(e => e.ProductsIngredients).
                    FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken: cancellationToken);
                if (ingredient == null)
                    return default;
                ingredient.Name = request.Name;
                ingredient.ProductsIngredients = GetProductIngredients(request.ProductsIngredients);
                await _context.SaveChangesAsync(cancellationToken);
                return ingredient.Id;
            }
        }
    }
}
