using CSharpFunctionalExtensions;

namespace Entities.Domain
{
    public class Menu : Ided// : Entity<int>
    {
        public ICollection<MenuItem>? MenuItems { get; set; }
    }
}
