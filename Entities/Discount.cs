using CSharpFunctionalExtensions;

namespace Entities
{
    public enum DiscountType { type1, type2 }
    public class Discount:Entity<int>
    {
        public DiscountType Type { get; set; }
        public decimal Size { get; set; }
    }
}