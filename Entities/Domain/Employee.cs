using CSharpFunctionalExtensions;

namespace Entities.Domain
{
    public class Employee : Entity<int>
    {
        public string? Name { get; set; }
    }
}
