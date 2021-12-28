using CSharpFunctionalExtensions;


namespace Entities.Domain
{
    public class Ingredient : Entity<int>
    {
        private string name = "Noname";
        public string Name { get => name; set { name = string.IsNullOrEmpty(value) || string.IsNullOrEmpty(value.Trim()) ? "Noname" : value; } }
        public int ProductId { get; set; }
    }
}