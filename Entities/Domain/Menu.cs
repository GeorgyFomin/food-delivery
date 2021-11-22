using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Menu : Entity<int>
    {
        public ICollection<MenuItem> MenuItems { get; set; }
    }
}
