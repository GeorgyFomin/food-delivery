using Entities.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using PhoneNumbers;
using UseCases.API.Dto;

namespace UseCases.API.Orders
{
    public class AddOrder
    {
        private static readonly PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();
        public class Command : IRequest<int>
        {
            public ICollection<OrderItemDto> OrderElements { get; set; } = new HashSet<OrderItemDto>();
            public DiscountDto? Discount { get; set; }
            public DeliveryDto? Delivery { get; set; }
            public ulong PhoneNumber { get; set; }
        }
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
                List<OrderItem> orderItems = new();
                List<OrderItemDto>? orderItemDtos = request.OrderElements.ToList();
                foreach (OrderItemDto item in orderItemDtos)
                {
                    if (item.Product != null)
                    {
                        Product? product = await _context.Products.Include(e => e.ProductsIngredients).
                            FirstOrDefaultAsync(p => p.Id == item.Product.Id, cancellationToken: cancellationToken);
                        OrderItem orderItem = new() { Product = product, Quantity = item.Quantity };
                        await _context.OrderItems.AddAsync(orderItem, cancellationToken);
                        await _context.SaveChangesAsync(cancellationToken);
                        orderItems.Add(orderItem);
                    }
                }
                Order order = new()
                {
                    Delivery = GetDelivery(request.Delivery),
                    Discount = GetDiscount(request.Discount),
                    OrderElements = orderItems,
                    PhoneNumder = GetPhoneNumber(request.PhoneNumber.ToString())
                    //Delivery = request.Delivery,
                    //Discount = request.Discount,
                    //OrderElements = orderItems,
                    //PhoneNumder = request.PhoneNumber
                };
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
