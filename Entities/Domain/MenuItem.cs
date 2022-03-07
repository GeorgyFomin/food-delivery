using CSharpFunctionalExtensions;

namespace Entities.Domain
{
    public class MenuItem : Entity<int>
    {
        public Product? Product { get; set; }
    }
}
