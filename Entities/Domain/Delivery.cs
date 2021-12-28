using CSharpFunctionalExtensions;


namespace Entities.Domain
{
    public class Delivery : Entity<int>
    {
        public string ServiceName { get; set; }
        public decimal Price { get; set; }
        public TimeSpan TimeSpan { get; set; }
    }
}
