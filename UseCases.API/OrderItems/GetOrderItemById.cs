﻿using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using UseCases.API.Dto;
using UseCases.API.Exceptions;

namespace UseCases.API.OrderItems
{
    public class GetOrderItemById
    {
        public class Query : IRequest<OrderItemDto?>
        {
            public int Id { get; set; }
        }
        public class QueryHandler : IRequestHandler<Query, OrderItemDto?>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public QueryHandler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<OrderItemDto?> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_context.OrderItems == null)
                {
                    return null;
                }
                var orderItem = await _context.OrderItems
                    .Include(e => e.Product)
                    .ThenInclude(p => p == null ? null : p.ProductsIngredients)
                    .FirstOrDefaultAsync(mi => mi.Id == request.Id, cancellationToken);
                if (orderItem == null)
                {
                    throw new EntityNotFoundException("OrderItem not found");
                }
                return _mapper.Map<OrderItemDto?>(orderItem);

            }
        }
    }
}
