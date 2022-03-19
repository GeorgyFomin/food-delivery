using Entities.Enums;

namespace UseCases.API.Dto
{
    public class DiscountDto
    {
        public int Id { get; set; }
        public DiscountType Type { get; set; }
        public decimal Size { get; set; }
    }
}
