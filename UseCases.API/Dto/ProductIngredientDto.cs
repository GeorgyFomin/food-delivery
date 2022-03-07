using System.Text.Json.Serialization;

namespace UseCases.API.Dto
{
    public class ProductIngredientDto
    {
        public int? ProductId { get; set; }
        [JsonIgnore]
        public ProductDto? Product { get; set; }
        public int IngredientId { get; set; }
        [JsonIgnore]
        public IngredientDto? Ingredient { get; set; }
    }
}
