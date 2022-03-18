using Entities.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseCases.API.Orders
{
    public class EditOrder
    {
        public class Command : IRequest<int>
        {
            public int Id { get; set; }
            public ICollection<OrderItem> OrderElements { get; set; } = new HashSet<OrderItem>();
            public Discount? Discount { get; set; }
            public Delivery? Delivery { get; set; }
        }
        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly DataContext _context;

            public CommandHandler(DataContext context) => _context = context;
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                if (_context.Orders == null)
                {
                    return default;
                }
                Order? order = await _context.Orders.Include(e => e.OrderElements).FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken: cancellationToken);
                if (order == null)
                {
                    return default;
                }
                order.Discount = request.Discount;
                order.Delivery = request.Delivery;
                order.OrderElements = request.OrderElements;
                await _context.SaveChangesAsync(cancellationToken);
                return order.Id;
            }
        }
    }
}
