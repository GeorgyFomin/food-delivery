using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    internal class Employee : Entity<Employee>
    {
        public int ID { get; set; }
        public string? Name { get; set; }
    }
}
