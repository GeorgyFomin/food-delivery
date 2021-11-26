using CSharpFunctionalExtensions;

namespace Entities
{
    public class Menu : Entity<int>
    {
        public ICollection<MenuItem> MenuItems { get; set; }
    }
}
