using CSharpFunctionalExtensions;

namespace Entities.Domain
{
    public class OrderItem : Entity<int>
    {
        public Product? Product { get; set; }
        public uint Quantity { get; set; }
    }
}
