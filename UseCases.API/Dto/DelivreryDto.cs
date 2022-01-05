using CSharpFunctionalExtensions;

namespace UseCases.API.Dto
{
    public class DeliveryDto : Entity<int>
    {
        //public int Id { get; set; }
        public string? ServiceName { get; set; }
        public decimal Price { get; set; }
        public TimeSpan TimeSpan { get; set; }
    }
}
