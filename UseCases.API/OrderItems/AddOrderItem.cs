using Entities.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using UseCases.API.Dto;

namespace UseCases.API.OrderItems
{
    public class AddOrderItem
    {
        public class Command : IRequest<int>
        {
            public ProductDto? Product { get; set; }
            public int Quantity { get; set; }
        }
        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly DataContext _context;
            public CommandHandler(DataContext context) => _context = context;
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request.Product == null) return default;
                Product? product = await _context.Products.Include(e => e.ProductsIngredients).
                    FirstOrDefaultAsync(p => p.Id == request.Product.Id, cancellationToken: cancellationToken);
                OrderItem orderItem = new() { Product = product, Quantity = request.Quantity };
                if (_context.OrderItems == null)
                {
                    return default;
                }
                await _context.OrderItems.AddAsync(orderItem, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return orderItem.Id;
            }
        }
    }
}
