using Entities;
using Entities.Domain;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;

namespace WebApi.Data
{
    public static class Seed
    {
        /// <summary>
        /// Хранит ссылку на генератор случайных чисел.
        /// </summary>
        static public readonly Random random = new();
        //private static readonly List<Ingredient> ingredients = new()
        //{
        //    new() { Name = "one", ProductId = null },
        //    new() { Name = "two", ProductId = null }
        //}; //GetRandomIngredients(random.Next(3, 6), random);
        private static readonly List<Product> products = GetRandomProducts(random.Next(3, 5), random);
        //static List<Ingredient> GetIngredients()
        //{
        //    List<Ingredient> list = new();
        //    for (int i = 0; i < (random.NextDouble() < .5 ? 1 : 2); i++)
        //    {
        //        list.Add(ingredients[i]);
        //    }
        //    return list;
        //    //return new(Enumerable.Range(0, ingredients.Count).Select(index => ingredients[random.Next(0, ingredients.Count - 1)]).ToList());
        //}
        /// <summary>
        /// Возвращает список случайных продуктов.
        /// </summary>
        /// <param name="v">Число продуктов.</param>
        /// <param name="random">Генератор случайных чисел.</param>
        /// <returns></returns>
        private static List<Product> GetRandomProducts(int v, Random random) => new(Enumerable.Range(0, v).
            Select(index => new Product()
            {
                Price = random.Next(1, 100),
                Weight = random.Next(1, 100),
                Name = GetRandomString(random.Next(1, 6), random),
                Ingredients = //GetIngredients()
                GetRandomIngredients(random.Next(1, 10), random)
            }).ToList());
        /// <summary>
        /// Возвращает список случайных ингредиентов.
        /// </summary>
        /// <param name="v">Число ингредиентов.</param>
        /// <param name="random">Генератор случайных чисел.</param>
        /// <returns></returns>
        private static List<Ingredient> GetRandomIngredients(int v, Random random) =>
                new(Enumerable.Range(0, v).Select(index => new Ingredient()
                {
                    Name = GetRandomString(random.Next(3, 6), random)
                }
            ).ToList());
        /// <summary>
        /// Генерирует случайную строку из латинских букв нижнего регистра..
        /// </summary>
        /// <param name="length">Длина строки.</param>
        /// <param name="random">Генератор случайных чисел.</param>
        /// <returns></returns>
        public static string GetRandomString(int length, Random random) => new(Enumerable.Range(0, length).Select(x => (char)random.Next('a', 'z' + 1)).ToArray());
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new DataContext(serviceProvider.GetRequiredService<DbContextOptions<DataContext>>());
            if (context == null || context.Products == null)
            {
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
                throw new ArgumentNullException("Null RazorDataContext");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
            }

            // Look for any departments.
            //if (context.Products.Any())
            //{
            //    return;   // DB has been seeded
            //}
            foreach (Ingredient ingredient in context.Ingredients)
            {
                context.Ingredients.Remove(ingredient);
            }
            foreach (Product product in context.Products)
            {
                context.Products.Remove(product);
            }
            context.Products.AddRange(products);
            //context.Ingredients.AddRange(ingredients);
            //foreach (Product product in products)
            //{
            //    List<Ingredient> ingredients1 = new(Enumerable.Range(0, ingredients.Count).Select(index => ingredients[random.Next(0, ingredients.Count - 1)]).ToList());
            //    product.Ingredients.AddRange(ingredients1);
            //}
            context.SaveChanges();
        }
    }
}
