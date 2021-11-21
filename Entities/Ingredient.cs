using CSharpFunctionalExtensions;

namespace Entities
{
    public class Ingredient:Entity<int>
    {
        public string Name { get; set; }
    }
}