using Entities.Domain;
using MediatR;
using Persistence.MsSql;

namespace UseCases.API.OrderItems
{
    public class EditOrderItem
    {
        public class Command : IRequest<int>
        {
            public int Id { get; set; }
            public Product? Product { get; set; }
            public int Quantity { get; set; }
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
                if (orderItem == null)
                {
                    return default;
                }
                orderItem.Product = request.Product;
                orderItem.Quantity = request.Quantity;
                await _context.SaveChangesAsync(cancellationToken);
                return orderItem.Id;
            }
        }
    }
}
