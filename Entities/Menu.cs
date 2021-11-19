using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    internal class Menu : Entity<Menu>
    {
        public int ID { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
