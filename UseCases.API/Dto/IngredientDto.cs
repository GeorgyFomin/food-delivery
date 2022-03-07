namespace UseCases.API.Dto
{
    public class IngredientDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Noname";
        public List<ProductIngredientDto>? ProductsIngredients { get; set; }
    }
}
