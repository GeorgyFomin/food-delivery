using CSharpFunctionalExtensions;
using System.ComponentModel.DataAnnotations;

namespace Entities.Domain
{
    public class Delivery : Entity<int>
    {
        [Required]
        public string? ServiceName { get; set; }
        public decimal Price { get; set; }
        public TimeSpan TimeSpan { get; set; }
    }
}
