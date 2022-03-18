using Entities.Domain;
using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Persistence.MsSql;

namespace WebApi.Data
{
    public static class DbSetExtension
    {
        public static void Clear<T>(this DbSet<T> dbSet) where T : class
        {
            if (dbSet.Any())
            {
                dbSet.RemoveRange(dbSet.ToList());
            }
        }
        public static void ClearSave<T>(this DbSet<T> dbSet, DataContext dataContext) where T : class
        {
            //if (dbSet==null)
            //{
            //    return;
            //}
            dbSet.Clear();
            dataContext.SaveChanges();
        }
    }
    public static class Seed
    {
        /// <summary>
        /// Хранит ссылку на генератор случайных чисел.
        /// </summary>
        public static readonly Random random = new();
        private static readonly List<Ingredient> ingredients = GetRandomIngredients(random.Next(5, 10));
        /// <summary>
        /// Возвращает список случайных ингредиентов.
        /// </summary>
        /// <param name="v">Число ингредиентов.</param>
        /// <returns></returns>
        private static List<Ingredient> GetRandomIngredients(int v) =>
                new(Enumerable.Range(0, v).Select(index => new Ingredient()
                {
                    Name = GetRandomString(random.Next(3, 6))
                }
            ).ToList());
        private static readonly List<Product> products = GetProducts();
        private static List<Product> GetProducts()
        {
            static List<Ingredient> GetIngredients()
            {
                List<Ingredient> list = new();
                // Число ингредиентов в списке.
                int nIngr = random.Next(2, ingredients.Count);
                Ingredient ingr;
                // Помещаем в список list разные случайно выбранные ингредиенты из полного списка ingredients в количестве nIngr.
                for (int i = 0; i < nIngr; i++)
                {
                    do
                    {
                        ingr = ingredients[random.Next(ingredients.Count)];
                    } while (list.Contains(ingr));
                    list.Add(ingr);
                }
                return list;
            }
            List<Product> products = GetRandomProducts(random.Next(3, 10));
            foreach (Product product in products)
            {
                List<Ingredient> ingredients = GetIngredients();
                List<ProductIngredient> productIngredients = new();
                foreach (Ingredient ingredient in ingredients)
                {
                    productIngredients.Add(new ProductIngredient() { Product = product, Ingredient = ingredient });
                }
                product.ProductsIngredients = productIngredients;
            }
            return products;
        }
        /// <summary>
        /// Возвращает список случайных продуктов.
        /// </summary>
        /// <param name="v">Число продуктов.</param>
        /// <returns></returns>
        private static List<Product> GetRandomProducts(int v) => new(Enumerable.Range(0, v).
            Select(index => new Product()
            {
                Price = random.Next(1, 100),
                Weight = random.Next(1, 100),
                Name = GetRandomString(random.Next(1, 6))
            }).ToList());
        private static List<MenuItem> GetRandomMenuItems(List<Product> products)
        {
            List<MenuItem> items = new();
            for (int i = 0; i < products.Count; i++)
            {
                if (random.NextDouble() < .9)
                {
                    items.Add(new MenuItem { Product = products[i] });
                }
            }
            if (items.Count == 0)
            {
                items.Add(new MenuItem { Product = products[random.Next(products.Count)] });
            }
            return items;
        }
        private static List<OrderItem> GetRandomOrderItems(List<Product> products)
        {
            List<OrderItem> items = new();
            for (int i = 0; i < products.Count; i++)
            {
                if (random.NextDouble() < .9)
                {
                    items.Add(new OrderItem { Product = products[i], Quantity = random.Next(1, 10) });
                }
            }
            if (items.Count == 0)
            {
                items.Add(new OrderItem { Product = products[random.Next(products.Count)], Quantity = random.Next(1, 10) });
            }
            return items;
        }
        private static Delivery GetRandomDelivery() => new() { ServiceName = GetRandomString(5), Price = random.Next(10), TimeSpan = GetRandomTimeSpan() };
        private static TimeSpan GetRandomTimeSpan() => new(random.Next(10), random.Next(60), random.Next(60));
        private static List<Delivery> GetRandomDeliveries(int v) => new(Enumerable.Range(0, v).Select(index => GetRandomDelivery()).ToList());
        private static readonly List<Delivery> deliveries = GetRandomDeliveries(random.Next(2, 5));
        private static List<Discount> GetRandomDiscounts(int v) =>
            new(Enumerable.Range(0, v).Select(index => new Discount { Size = random.Next(10), Type = random.NextDouble() < .5 ? DiscountType.percentage : DiscountType.absolute }).ToList());
        private static readonly List<Discount> discounts = GetRandomDiscounts(random.Next(3, 10));
        private static readonly List<MenuItem> menuItems = GetRandomMenuItems(products);
        private static List<Menu> GetRandomMenus(int v)
        {
            static List<MenuItem> GetMenuItems()
            {
                List<MenuItem> list = new();
                // Число элементов меню в списке.
                int nMenuItems = random.Next(1, menuItems.Count);
                MenuItem menuItem;
                // Помещаем в список list разные случайно выбранные элементы меню из полного списка элементов меню.
                for (int i = 0; i < nMenuItems; i++)
                {
                    do
                    {
                        menuItem = menuItems[random.Next(menuItems.Count)];
                    } while (list.Contains(menuItem));
                    list.Add(menuItem);
                }
                return list;
            }
            List<Menu> list = new(Enumerable.Range(0, v).Select(Index => new Menu()).ToList());
            for (int i = 0; i < list.Count; i++)
            {
                List<MenuItem> menuItems = GetMenuItems();
                list[i].MenuItems = menuItems;
            }
            return list;
        }
        private static readonly List<Menu> menus = GetRandomMenus(random.Next(2, 5));
        private static readonly List<OrderItem> orderItems = GetRandomOrderItems(products);
        private static List<Order> GetRandomOrders(int v)
        {
            static List<OrderItem> GetOrderItems()
            {
                List<OrderItem> list = new();
                // Число элементов заказов в списке.
                int nOrderItems = random.Next(1, orderItems.Count);
                OrderItem orderItem;
                // Помещаем в список list разные случайно выбранные элементы заказа из полного списка элементов заказа.
                for (int i = 0; i < nOrderItems; i++)
                {
                    do
                    {
                        orderItem = orderItems[random.Next(orderItems.Count)];
                    } while (list.Contains(orderItem));
                    list.Add(orderItem);
                }
                return list;
            }
            List<Order> list = new(Enumerable.Range(0, v).Select(Index => new Order()).ToList());
            for (int i = 0; i < list.Count; i++)
            {
                List<OrderItem> orderItems = GetOrderItems();
                list[i].OrderElements = orderItems;
                list[i].Delivery = deliveries[random.Next(deliveries.Count)];
                list[i].Discount = discounts[random.Next(discounts.Count)];
            }
            return list;
        }
        private static readonly List<Order> orders = GetRandomOrders(random.Next(2, 5));
        /// <summary>
        /// Генерирует случайную строку из латинских букв нижнего регистра.
        /// </summary>
        /// <param name="length">Длина строки.</param>
        /// <returns></returns>
        private static string GetRandomString(int length) => new(Enumerable.Range(0, length).Select(x => (char)random.Next('a', 'z' + 1)).ToArray());
        private static List<Employee> GetRandomEmployees(int v) => new(Enumerable.Range(0, v).Select(index => new Employee { Name = GetRandomString(random.Next(2, 5)) }));
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new DataContext(serviceProvider.GetRequiredService<DbContextOptions<DataContext>>());
            if (context == null) return;
            // Чистим таблицы.
            context.Employees.ClearSave(context);
            context.Ingredients.ClearSave(context);
            //context.MenuItems.ClearSave(context);
            //context.Menus.ClearSave(context);
            //context.OrderItems.ClearSave(context);
            //context.Orders.ClearSave(context);
            context.Deliveries.ClearSave(context);
            context.Discounts.ClearSave(context);
            context.Products.ClearSave(context);
            // Заполняем таблицы случайными полями.
            context.Ingredients.AddRange(ingredients);
            context.Products.AddRange(products);
            //context.OrderItems.AddRange(orderItems);
            //context.MenuItems.AddRange(menuItems);
            //context.Menus.AddRange(menus);
            context.Deliveries.AddRange(deliveries);
            context.Discounts.AddRange(discounts);
            //context.Orders.AddRange(orders);
            context.Employees.AddRange(GetRandomEmployees(random.Next(3, 7)));
            // Сохраняем таблицы в базе.
            context.SaveChanges();
        }

    }
}
