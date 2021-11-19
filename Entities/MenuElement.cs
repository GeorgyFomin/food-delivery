﻿using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class MenuElement:Entity<MenuElement>
    {
        public int ID { get; set; }
        public Product? Product { get; set; }
    }
}