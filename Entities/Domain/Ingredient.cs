using CSharpFunctionalExtensions;
using System.ComponentModel.DataAnnotations;

namespace Entities.Domain
{
    public class Ingredient : Entity<int>
    {
        [Required]
        public string? Name { get; set; }
        //public ICollection<Product>? Products { get; set; }
        // Foreign Key
        public int? ProductId { get; set; }
    }
}