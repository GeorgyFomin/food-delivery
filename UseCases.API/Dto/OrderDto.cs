namespace UseCases.API.Dto
{
    public class OrderDto
    {
        public int Id { get; set; }
        public ICollection<OrderItemDto> OrderElements { get; set; } = new HashSet<OrderItemDto>();
        public DiscountDto? Discount { get; set; }
        public DeliveryDto? Delivery { get; set; }
    }
}
