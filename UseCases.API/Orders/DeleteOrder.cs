﻿using Entities.Domain;
using MediatR;
using Persistence.MsSql;
namespace UseCases.API.Orders
{
    public class DeleteOrder
    {
        public class Command : IRequest
        {
            public int Id { get; set; }
        }
        public class CommandHandler : IRequestHandler<Command, Unit>
        {
            private readonly DataContext _context;
            public CommandHandler(DataContext context) => _context = context;
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                if (_context.Orders == null)
                {
                    return Unit.Value;
                }
                Order? order = await _context.Orders.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
                if (order == null) return Unit.Value;
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}
