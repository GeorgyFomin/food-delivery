using CSharpFunctionalExtensions;

using System.ComponentModel.DataAnnotations;
//#nullable disable
namespace Entities.Domain
{
    public class Product : Named//: Entity<int>
    {
        //public int Id { get; set; }
        [DisplayFormat(DataFormatString = "{0:n0}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }
        [DisplayFormat(DataFormatString = "{0:n0}", ApplyFormatInEditMode = true)]
        public double Weight { get; set; }
        public virtual ICollection<Ingredient>? Ingredients { get; set; }
    }
}