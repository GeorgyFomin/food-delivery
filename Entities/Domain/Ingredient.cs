using CSharpFunctionalExtensions;
using Entities.Domain;

namespace Entities
{
    public class Ingredient : Named //:Entity<int>
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
    }
}