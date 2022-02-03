using CSharpFunctionalExtensions;

using System.ComponentModel.DataAnnotations;

namespace Entities.Domain
{
    public class Product : Entity<int>
    {
        [Required]
        public string? Name { get; set; }
        [DisplayFormat(DataFormatString = "{0:n0}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }
        [DisplayFormat(DataFormatString = "{0:n0}", ApplyFormatInEditMode = true)]
        public double Weight { get; set; }
        public virtual ICollection<Ingredient>? Ingredients { get; set; }
    }
}