using CSharpFunctionalExtensions;
using System.ComponentModel.DataAnnotations;

namespace Entities.Domain
{
    public class Product : Entity<int>
    {
        public string Name { get; set; } = "Noname";
        [DisplayFormat(DataFormatString = "{0:n0}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }
        [DisplayFormat(DataFormatString = "{0:n0}", ApplyFormatInEditMode = true)]
        public double Weight { get; set; }
        public ICollection<ProductIngredient> ProductsIngredients { get; set; } = new List<ProductIngredient>();
    }
}