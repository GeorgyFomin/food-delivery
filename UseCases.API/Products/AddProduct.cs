using Entities;
using MediatR;
using Persistence.MsSql;

namespace UseCases.API.Products
{
    public class AddProduct
    {
        public class Command:IRequest<int>
        {
            public decimal Price { get; set; }
            public string Name { get; set; }
            public double Weight { get; set; }
            public ICollection<Ingredient> Ingredients { get; set; }
        }
        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly DataContext _context;

            public CommandHandler(DataContext context) => _context = context;
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                Product product = new() { Ingredients = request.Ingredients, Name = request.Name, Price = request.Price, Weight = request.Weight };
                await _context.Products.AddAsync(product, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return product.Id;
            }
        }
    }
}
