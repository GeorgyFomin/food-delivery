using System.ComponentModel.DataAnnotations;

namespace UseCases.API.Dto
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Noname";
        [DisplayFormat(DataFormatString = "{0:n0}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }
        [DisplayFormat(DataFormatString = "{0:n0}", ApplyFormatInEditMode = true)]
        public double Weight { get; set; }
        public ICollection<ProductIngredientDto> ProductsIngredients { get; set; } = new List<ProductIngredientDto>();
    }
}
