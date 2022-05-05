using Entities.Domain;

namespace UseCases.API.Dto
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public ProductDto? Product { get; set; }
        public int Quantity { get; set; }
    }
}
