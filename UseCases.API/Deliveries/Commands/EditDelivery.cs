﻿using Entities;
using Entities.Domain;
using MediatR;
using Persistence.MsSql;
namespace UseCases.API.Deliveries
{
    public class EditDelivery
    {
        public class Command : IRequest<int>
        {
            public int Id { get; set; }
            public string ServiceName { get; set; } = "Noname";
            public decimal Price { get; set; }
            public TimeSpan TimeSpan { get; set; }
        }
        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly DataContext _context;

            public CommandHandler(DataContext context) => _context = context;
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                if (_context.Deliveries==null)
                {
                    return default; 
                }
                Delivery? delivery = await _context.Deliveries.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
                if (delivery == null)
                    return default;
                delivery.ServiceName = request.ServiceName;
                delivery.Price = request.Price;
                delivery.TimeSpan = request.TimeSpan;
                await _context.SaveChangesAsync(cancellationToken);
                return delivery.Id;
            }
        }
    }
}
