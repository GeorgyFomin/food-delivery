using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    internal class Order : Entity<Order>
    {
        public int ID { get; set; }
        public int Number { get; set; }
        public Product? Product { get; set; }
    }
}
