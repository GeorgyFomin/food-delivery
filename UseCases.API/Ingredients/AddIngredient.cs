using Entities.Domain;
using MediatR;
using Persistence.MsSql;
using UseCases.API.Dto;

namespace UseCases.API.Ingredients
{
    public class AddIngredient
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
            public string Name { get; set; } = "Noname";
            public List<ProductIngredientDto>? ProductsIngredients { get; set; }
        }
        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly DataContext _context;

            public CommandHandler(DataContext context) => _context = context;
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                Ingredient Ingredient = new()
                {
                    Name = request.Name,
                    ProductsIngredients = GetProductIngredients(request.ProductsIngredients)
                };
                if (_context.Ingredients == null)
                {
                    return default;
                }
                await _context.Ingredients.AddAsync(Ingredient, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Ingredient.Id;
            }
        }
    }
}
