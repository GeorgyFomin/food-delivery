using CSharpFunctionalExtensions;

namespace Entities.Domain
{
    public class Menu : Entity<int>
    {
        public ICollection<MenuItem> MenuItems { get; set; } = new HashSet<MenuItem>();
    }
}
