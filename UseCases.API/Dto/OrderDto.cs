using Newtonsoft.Json;
using PhoneNumbers;

namespace UseCases.API.Dto
{
    public class OrderDto
    {
        public int Id { get; set; }
        public ICollection<OrderItemDto> OrderElements { get; set; } = new List<OrderItemDto>();
        public DiscountDto? Discount { get; set; } = new();
        public DeliveryDto? Delivery { get; set; } = new();
        public ulong PhoneNumber { get; set; }
    }
}
