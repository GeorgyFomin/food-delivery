using CSharpFunctionalExtensions;
using Entities.Domain;

namespace UseCases.API.Dto
{
    public class IngredientDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        //public ICollection<Product>? Products { get; set; }
        //public Product? Product { get; set; }
        public int? ProductId { get; set; }
    }
}
