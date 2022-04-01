namespace UseCases.API.Dto
{
    public class MenuDto
    {
        public int Id { get; set; }
        public List<MenuItemDto> MenuItems { get; set; } = new();
    }
}
