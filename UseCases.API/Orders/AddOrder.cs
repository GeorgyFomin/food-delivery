using Entities.Domain;
using MediatR;
using Persistence.MsSql;

namespace UseCases.API.Orders
{
    public class AddOrder
    {
        public class Command : IRequest<int>
        {
            public ICollection<OrderItem>? OrderElements { get; set; }
            public Discount? Discount { get; set; }
            public Delivery? Delivery { get; set; }
        }
        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly DataContext _context;
            public CommandHandler(DataContext context) => _context = context;
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                Order order = new() { Delivery = request.Delivery, Discount = request.Discount, OrderElements = request.OrderElements };
                if (_context.Orders == null)
                {
                    return default;
                }
                await _context.Orders.AddAsync(order, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return order.Id;
            }
        }
    }
}
