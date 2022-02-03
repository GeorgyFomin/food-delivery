using Entities;
using Entities.Domain;
using MediatR;
using Persistence.MsSql;

namespace UseCases.API.Ingredients
{
    public class EditIngredient
    {
        public class Command : IRequest<int>
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            //public ICollection<Product>? Products { get; set; }
            //public Product? Product { get; set; }
            public int? ProductId { get; set; }
        }
        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly DataContext _context;

            public CommandHandler(DataContext context) => _context = context;
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                Ingredient? ingredient = await _context.Ingredients.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
                if (ingredient == null)
                    return default;
                ingredient.Name = request.Name;
                ingredient.ProductId = request.ProductId;
                //ingredient.Product = request.Product;
                //ingredient.Products = request.Products;
                await _context.SaveChangesAsync(cancellationToken);
                return ingredient.Id;
            }
        }
    }
}
