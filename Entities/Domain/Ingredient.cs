using CSharpFunctionalExtensions;


namespace Entities.Domain
{
    public class Ingredient : Named //:Entity<int>
    {
        public int ProductId { get; set; }
    }
}