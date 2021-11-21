using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class MenuElement:Entity<int>
    {
        public Product Product { get; set; }
    }
}
