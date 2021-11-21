using CSharpFunctionalExtensions;
using Entities.Enums;

namespace Entities
{
    public class Discount:Entity<int>
    {
        public DiscountType Type { get; set; }
        public decimal Size { get; set; }
    }
}