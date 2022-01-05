using CSharpFunctionalExtensions;
using Entities.Domain;

namespace UseCases.API.Dto
{
    public class ProductDto : Entity<int>
    {
        //public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public double Weight { get; set; }
        public virtual ICollection<Ingredient>? Ingredients { get; set; }
    }
}
