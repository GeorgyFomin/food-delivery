﻿using CSharpFunctionalExtensions;

namespace WebASP_MVC.Models
{
    public class Delivery //: Entity<int>
    {
        public int Id { get; set; }
        public string? ServiceName { get; set; }
        public decimal Price { get; set; }
        public TimeSpan TimeSpan { get; set; }
    }
}