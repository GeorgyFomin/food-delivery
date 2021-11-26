using CSharpFunctionalExtensions;

namespace Entities
{
    public class MenuItem:Entity<int>
    {
        public Product Product { get; set; }
    }
}
