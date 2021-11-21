using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Order : Entity<int>
    {
        public ICollection<OrderElement> OrderElements { get; set; }
        public Discount Discount { get; set; }
        public Delivery Delivery { get; set; }
    }
}
