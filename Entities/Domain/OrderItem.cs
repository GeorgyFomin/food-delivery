using CSharpFunctionalExtensions;

namespace Entities
{
    public class OrderItem : Entity<int>
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
