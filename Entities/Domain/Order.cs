using CSharpFunctionalExtensions;
using PhoneNumbers;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Domain
{
    public class Order : Entity<int>
    {
        public ICollection<OrderItem> OrderElements { get; set; } = new HashSet<OrderItem>();
        public Discount? Discount { get; set; }
        public Delivery? Delivery { get; set; }
        public PhoneNumber? PhoneNumder { get; set; }
    }
}
