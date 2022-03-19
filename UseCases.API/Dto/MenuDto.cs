namespace UseCases.API.Dto
{
    public class MenuDto
    {
        public ICollection<MenuItemDto> MenuItems { get; set; } = new HashSet<MenuItemDto>();
    }
}
