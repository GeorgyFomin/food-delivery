using CSharpFunctionalExtensions;
using Entities.Enums;

namespace Entities.Domain
{
    public class Discount : Ided// : Entity<int>
    {
        public DiscountType Type { get; set; }
        public decimal Size { get; set; }
    }
}