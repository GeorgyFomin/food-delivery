using CSharpFunctionalExtensions;
#nullable disable
namespace Entities
{
    public class Product : Entity<Product>
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}