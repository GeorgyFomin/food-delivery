using AutoMapper;
using Entities.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using UseCases.API.Dto;
using UseCases.API.Exceptions;

namespace UseCases.API.OrderItems
{
    public class GetOrderItems
    {
        public class Query : IRequest<IEnumerable<OrderItemDto>> { }
        public class QueryHandler : IRequestHandler<Query, IEnumerable<OrderItemDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public QueryHandler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<OrderItemDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_context.OrderItems == null)
                {
                    return Enumerable.Empty<OrderItemDto>();
                }
                var orderItems = await _context.OrderItems.Include(e => e.Product).ThenInclude(p => p != null ? p.ProductsIngredients : null).ToListAsync(cancellationToken);
                if (orderItems == null)
                {
                    throw new EntityNotFoundException("OrderItems not found");
                }
                // Если среди всех элементов заказа есть такие, которые не входят ни в один из заказов, то они убираются из базы.
                List<OrderItem>?
                    // Элементы заказа, не входит ни в один из заказов.
                    nullItems = new(),
                    // Элементы заказа, объединяющие все элемениты всех заказов.
                    allItems = new();
                // Собираем элементы всех заказов.
                foreach (Order order in _context.Orders)
                    allItems.AddRange(order.OrderElements);
                // Проходим по всем элементам заказов.
                foreach (OrderItem orderItem in orderItems)
                {
                    // Если данный элемент заказа не входит ни в один заказ, то он добавляется к nullItems.
                    if (allItems.Find(oi => oi.Id == orderItem.Id) == null)
                    {
                        nullItems.Add(orderItem);
                    }
                }
                // Из базы элементов заказов удаляются все элементы, которые не входят ни в один из заказов.
                _context.OrderItems.RemoveRange(nullItems);
                await _context.SaveChangesAsync();
                return _mapper.Map<List<OrderItemDto>>(await _context.OrderItems.ToListAsync(cancellationToken));
            }
        }
    }
}
