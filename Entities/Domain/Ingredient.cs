using CSharpFunctionalExtensions;
using System.ComponentModel.DataAnnotations;

namespace Entities.Domain
{
    public class Ingredient : Entity<int>
    {
        [Required]
        public string Name { get; set; }
        // Foreign Key
        public int ProductId { get; set; }
    }
}