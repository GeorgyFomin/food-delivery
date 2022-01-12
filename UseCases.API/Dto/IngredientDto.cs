using CSharpFunctionalExtensions;

namespace UseCases.API.Dto
{
    public class IngredientDto 
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? ProductId { get; set; }
    }
}
