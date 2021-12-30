using CSharpFunctionalExtensions;


namespace Entities.Domain
{
    public class Ingredient : Entity<int>
    {
        public string Name { get; set; }
    }
}