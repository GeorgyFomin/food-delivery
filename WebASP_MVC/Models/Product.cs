namespace WebASP_MVC.Models
{
    public class Product
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
        public double Weight { get; set; }
        public ICollection<Ingredient> Ingredients { get; set; }
    }
}
