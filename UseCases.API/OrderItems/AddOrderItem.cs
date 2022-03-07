using Entities.Domain;
using MediatR;
using Persistence.MsSql;

namespace UseCases.API.OrderItems
{
    public class AddOrderItem
    {
        public class Command : IRequest<int>
        {
            public Product? Product { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
        }
        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly DataContext _context;
            public CommandHandler(DataContext context) => _context = context;
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                OrderItem orderItem = new() { Price = request.Price, Product = request.Product, Quantity = request.Quantity };
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
