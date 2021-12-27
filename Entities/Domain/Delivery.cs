using CSharpFunctionalExtensions;


namespace Entities.Domain
{
    public class Delivery:Ided //: Entity<int>
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public decimal Price { get; set; }
        public TimeSpan TimeSpan { get; set; }
    }
}
