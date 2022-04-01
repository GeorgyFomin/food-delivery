using Entities.Domain;

namespace UseCases.API.Dto
{
    public class MenuItemDto
    {
        public int Id { get; set; }
        //public Product? Product { get; set; }
        public ProductDto? Product { get; set; }
    }
}
