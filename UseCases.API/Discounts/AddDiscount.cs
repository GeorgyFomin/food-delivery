using Entities.Domain;
using Entities.Enums;
using MediatR;
using Persistence.MsSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseCases.API.Discounts
{
    public class AddDiscount
    {
        public class Command : IRequest<int>
        {
            public DiscountType Type { get; set; }
            public decimal Size { get; set; }
        }
        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly DataContext _context;

            public CommandHandler(DataContext context) => _context = context;
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                if (_context.Discounts == null)
                {
                    return default;
                }
                Discount discount = new() { Type = request.Type, Size = request.Size };
                await _context.Discounts.AddAsync(discount, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return discount.Id;
            }
        }
    }
}
