using Entities;
using MediatR;
using Persistence.MsSql;

namespace UseCases.API.Products
{
    public class EditProduct
    {
        public class Command : IRequest<int>
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public double Weight { get; set; }
            public ICollection<Ingredient>? Ingredients { get; set; }
        }
        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly DataContext _context;

            public CommandHandler(DataContext context) => _context = context;
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                Product? product = await _context.Products.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
                if (product == null)
                    return default;
                product.Name = request.Name;
                product.Price = request.Price;
                product.Weight = request.Weight;
                product.Ingredients = request.Ingredients;
                await _context.SaveChangesAsync(cancellationToken);
                return product.Id;
            }
        }
    }
}
