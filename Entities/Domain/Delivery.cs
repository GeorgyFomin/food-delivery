using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Delivery : Entity<int>
    {
        public string ServiceName { get; set; }
        public decimal Price { get; set; }
        public TimeSpan TimeSpan { get; set; }
    }
}
