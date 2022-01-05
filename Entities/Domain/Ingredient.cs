using CSharpFunctionalExtensions;
using System.ComponentModel.DataAnnotations;

namespace Entities.Domain
{
    public class Ingredient : Entity<int>
    {
        //public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        // Foreign Key
        public int ProductId { get; set; }
    }
}