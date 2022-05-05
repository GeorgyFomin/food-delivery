using Entities.Domain;
using MediatR;
using Persistence.MsSql;
using UseCases.API.Dto;

namespace UseCases.API.Products
{
    public class AddProduct
    {
        public class Command : IRequest<int>
        {
            public string Name { get; set; } = "Noname";
            public decimal Price { get; set; }
            public double Weight { get; set; }
            public ICollection<ProductIngredientDto> ProductsIngredients { get; set; } = new List<ProductIngredientDto>();
        }
        static List<ProductIngredient> GetProductIngredients(List<ProductIngredientDto> productIngredientDtos)
        {
            List<ProductIngredient> productIngredients = new();
            foreach (ProductIngredientDto productIngredientDto in productIngredientDtos)
            {
                productIngredients.Add(new ProductIngredient() { IngredientId = productIngredientDto.IngredientId, ProductId = productIngredientDto.ProductId });
            }
            return productIngredients;
        }

        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly DataContext _context;

            public CommandHandler(DataContext context) => _context = context;
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                Product? product = new()
                {
                    Name = request.Name,
                    Price = request.Price,
                    Weight = request.Weight,
                    ProductsIngredients = GetProductIngredients(new List<ProductIngredientDto>(request.ProductsIngredients)) 
                };
                if (_context.Products == null)
                    return default;
                await _context.Products.AddAsync(product, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return product.Id;
            }
        }
    }
}
