using CSharpFunctionalExtensions;

namespace Entities.Domain
{
    public class Menu : Entity<int>
    {
        public List<MenuItem> MenuItems { get; set; } = new();
    }
}
