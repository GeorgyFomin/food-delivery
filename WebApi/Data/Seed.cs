using Entities;
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
        private static readonly List<Product> Products = GetRandomProducts(random.Next(1, 5), random);
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
                Ingredients = GetRandomIngredients(random.Next(1, 20), random)
            }).ToList());
        /// <summary>
        /// Возвращает список случайных ингредиентов.
        /// </summary>
        /// <param name="v">Число ингредиентов.</param>
        /// <param name="random">Генератор случайных чисел.</param>
        /// <returns></returns>
        private static List<Ingredient> GetRandomIngredients(int v, Random random) => new(Enumerable.Range(0, v).Select(index => new Ingredient()
        {
            Name = GetRandomString(random.Next(3, 6), random)
        }).ToList());
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
            context.Products.AddRange(Products);
            context.SaveChanges();
        }
    }
}
