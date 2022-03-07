using CSharpFunctionalExtensions;
namespace Entities.Domain
{
    public class Ingredient : Entity<int>
    {
        public string Name { get; set; } = "Noname";
        public List<ProductIngredient>? ProductsIngredients { get; set; }

    }
}