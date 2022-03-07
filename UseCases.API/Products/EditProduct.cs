using Entities.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;

namespace UseCases.API.Products
{
    public class EditProduct
    {
        public class Command : IRequest<int>
        {
            public int Id { get; set; }
            public string Name { get; set; } = "Noname";
            public decimal Price { get; set; }
            public double Weight { get; set; }
            public ICollection<ProductIngredient> ProductsIngredients { get; set; } = new List<ProductIngredient>();
        }
        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly DataContext _context;

            public CommandHandler(DataContext context) => _context = context;
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                if (_context.Products==null)
                {
                    return default;
                }
                Product? product = await _context.Products.Include(e => e.ProductsIngredients).
                    FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken: cancellationToken);
                if (product == null)
                    return default;
                product.Name = request.Name;
                product.Price = request.Price;
                product.Weight = request.Weight;
                product.ProductsIngredients = request.ProductsIngredients;
                await _context.SaveChangesAsync(cancellationToken);
                return product.Id;
            }
        }
    }
}
