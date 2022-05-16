namespace UseCases.API.Dto
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public ProductDto? Product { get; set; } = new ProductDto();
        public uint Quantity { get; set; }
    }
}
