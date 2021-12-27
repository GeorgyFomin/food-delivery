using CSharpFunctionalExtensions;


namespace Entities.Domain
{
    public class Order : Ided //Entity<int>
    {
        public ICollection<OrderItem> OrderElements { get; set; }
        public Discount Discount { get; set; }
        public Delivery Delivery { get; set; }
    }
}
