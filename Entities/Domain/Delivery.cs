using CSharpFunctionalExtensions;
namespace Entities.Domain
{
    public class Delivery : Entity<int>
    {
        public string ServiceName { get; set; } = "Noname";
        public decimal Price { get; set; }
        public TimeSpan TimeSpan { get; set; }
    }
}
