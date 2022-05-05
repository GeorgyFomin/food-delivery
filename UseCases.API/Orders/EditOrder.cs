using Entities.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using PhoneNumbers;
using UseCases.API.Dto;

namespace UseCases.API.Orders
{
    public class EditOrder
    {
        private static readonly PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();
        static PhoneNumber GetPhoneNumber(string phNumber)
        {
            if (string.IsNullOrWhiteSpace(phNumber) || phNumber.Length < 2 || phNumber.Length > 10 || !ulong.TryParse(phNumber, out _))
            {
                phNumber = "10";
            }
            return phoneUtil.Parse("+7" + phNumber, "ru");
        }
        static Delivery? GetDelivery(DeliveryDto? deliveryDto) => deliveryDto == null ? null : new()
        {
            Price = deliveryDto.Price,
            ServiceName = string.IsNullOrWhiteSpace(deliveryDto.ServiceName) ? "Noname" : deliveryDto.ServiceName,
            TimeSpan = deliveryDto.TimeSpan
        };
        static Discount? GetDiscount(DiscountDto? discountDto) => discountDto == null ? null : new()
        {
            Size = discountDto.Size,
            Type = discountDto.Type
        };
        public class Command : IRequest<int>
        {
            public int Id { get; set; }
            public ICollection<OrderItemDto> OrderElements { get; set; } = new HashSet<OrderItemDto>();
            public DiscountDto? Discount { get; set; }
            public DeliveryDto? Delivery { get; set; }
            public ulong PhoneNumber { get; set; }
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
                List<OrderItem> orderItems = order.OrderElements.ToList();
                OrderItemDto? item = request.OrderElements.ToList().Find(oi => oi.Id == 0);
                if (item != null && item.Product != null)
                {
                    Product? product = await _context.Products.Include(e => e.ProductsIngredients).
                        FirstOrDefaultAsync(p => p.Id == item.Product.Id, cancellationToken: cancellationToken);
                    OrderItem orderItem = new() { Product = product, Quantity = item.Quantity };
                    if (_context.OrderItems == null)
                    {
                        return default;
                    }
                    await _context.OrderItems.AddAsync(orderItem, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                    orderItems.Add(orderItem);
                }
                order.Discount = GetDiscount(request.Discount);
                order.Delivery = GetDelivery(request.Delivery);
                order.OrderElements = orderItems;
                order.PhoneNumder = GetPhoneNumber(request.PhoneNumber.ToString());
                await _context.SaveChangesAsync(cancellationToken);
                return order.Id;
            }
        }
    }
}
