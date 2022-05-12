using Entities.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using UseCases.API.Dto;

namespace UseCases.API.OrderItems
{
    public class EditOrderItem
    {
        public class Command : IRequest<int>
        {
            public int Id { get; set; }
            public ProductDto? Product { get; set; }
            public uint Quantity { get; set; }
        }
        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly DataContext _context;

            public CommandHandler(DataContext context) => _context = context;
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                if (_context.OrderItems == null)
                {
                    return default;
                }
                OrderItem? orderItem = await _context.OrderItems.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
                if (orderItem == null || request.Product == null)
                {
                    return default;
                }
                Product? product = await _context.Products.Include(e => e.ProductsIngredients).
                    FirstOrDefaultAsync(p => p.Id == request.Product.Id, cancellationToken: cancellationToken);
                if (product == null)
                {
                    return default;
                }
                orderItem.Product = product;
                orderItem.Quantity = request.Quantity;
                await _context.SaveChangesAsync(cancellationToken);
                return orderItem.Id;
            }
        }
    }
}
