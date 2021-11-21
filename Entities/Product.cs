using CSharpFunctionalExtensions;
#nullable disable
namespace Entities
{
    public class Product : Entity<int>
    {
        public decimal Price { get; set; }
        public string Name { get; set; }
        public double Weight { get; set; }
        public ICollection<Ingredient> Ingredients { get; set; }
    }
}