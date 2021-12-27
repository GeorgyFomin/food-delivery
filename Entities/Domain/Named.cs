using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domain
{
    public class Named
    {
        private string name = "Noname";
        public string Name { get => name; set { name = string.IsNullOrEmpty(value) || string.IsNullOrEmpty(value.Trim()) ? "Noname" : value; } }
        //public string Name { get; set; }
    }
}
