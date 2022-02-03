using Entities.Domain;
using MediatR;
using Persistence.MsSql;


namespace UseCases.API.Ingredients
{
    public class AddIngredient
    {
        public class Command : IRequest<int>
        {
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
                Ingredient Ingredient = new() { Name = request.Name, ProductId = request.ProductId }; //Products = request.Products };
                await _context.Ingredients.AddAsync(Ingredient, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Ingredient.Id;
            }
        }
    }
}
