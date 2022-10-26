using Entities.Domain;
using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using PhoneNumbers;

namespace WebApi.Data
{
    public static class DbSetExtension
    {
        public static void ClearSave<T>(this DbSet<T> dbSet, DataContext dataContext) where T : class
        {
            if (dbSet.Any())
            {
                dbSet.RemoveRange(dbSet.ToList());
                dataContext.SaveChanges();
            }
        }
    }
    public static class Seed
    {
        /// <summary>
        /// Хранит ссылку на генератор случайных чисел.
        /// </summary>
        public static readonly Random random = new();
        private static readonly List<Ingredient> ingredients = GetRandomIngredients(random.Next(5, 10));
        private static readonly PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();
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
            ));
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
            }));
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
                    items.Add(new OrderItem { Product = products[i], Quantity = (uint)random.Next(1, 10) });
                }
            }
            if (items.Count == 0)
            {
                items.Add(new OrderItem { Product = products[random.Next(products.Count)], Quantity = (uint)random.Next(1, 10) });
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
            List<Menu> list = new(Enumerable.Range(0, v).Select(Index => new Menu() { MenuItems = GetMenuItems() }).ToList());
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
                list[i].OrderElements = GetOrderItems();
                list[i].Delivery = deliveries[random.Next(deliveries.Count)];
                list[i].Discount = discounts[random.Next(discounts.Count)];
                list[i].PhoneNumder = GetRandomPhoneNumber();
            }
            return list;
        }
        private static PhoneNumber? GetRandomPhoneNumber() =>
            phoneUtil.Parse("+7" + new string(Enumerable.Range(2, random.Next(2, 11)).Select(x => (char)random.Next('0', '9' + 1)).ToArray()), "ru");
        private static readonly List<Order> orders = GetRandomOrders(random.Next(2, 5));
        /// <summary>
        /// Генерирует случайную строку из латинских букв нижнего регистра.
        /// </summary>
        /// <param name="length">Длина строки.</param>
        /// <returns></returns>
        private static string GetRandomString(int length) => new(Enumerable.Range(0, length).Select(x => (char)random.Next('a', 'z' + 1)).ToArray());
        private static List<Employee> GetRandomEmployees(int v) => new(Enumerable.Range(0, v).Select(index => new Employee { Name = GetRandomString(random.Next(2, 5)) }));
        //private static List<ApplicationUser> GetRandomUsers(int v)
        //{
        //    return new(Enumerable.Range(0, v).Select(index =>
        //    {
        //        string
        //            userName = GetRandomString(random.Next(2, 5)),
        //            eMail = GetRandomString(random.Next(2, 5)) + "@" + GetRandomString(random.Next(2, 5)) + ".com";
        //        return new ApplicationUser
        //        {
        //            UserName = userName,
        //            NormalizedUserName = userName.ToUpper(),
        //            Alias = GetRandomString(random.Next(2, 5)),
        //            PhoneNumber = "+7" + (string)new(Enumerable.Range(2, random.Next(2, 11)).Select(x => (char)random.Next('0', '9' + 1)).ToArray()),
        //            PhoneNumberConfirmed = true,
        //            Email = eMail,
        //            NormalizedEmail = eMail.ToUpper(),
        //            EmailConfirmed = true
        //        };
        //    }));
        //}
        //private static List<IdentityRole> GetRandomRoles(int v)
        //{
        //    return new(Enumerable.Range(0, v).Select(index =>
        //    {
        //        string name = GetRandomString(random.Next(2, 5));
        //        return new IdentityRole
        //        {
        //            Name = name,
        //            NormalizedName = name.ToUpper(),
        //            ConcurrencyStamp = Guid.NewGuid().ToString()
        //        };
        //    }));
        //}
        //private static readonly List<IdentityRole> roles = GetRandomRoles(random.Next(5, 10));
        //private static readonly List<ApplicationUser> users = GetRandomUsers(random.Next(3, 10));

        ////private static readonly List<ApplicationUser> users = GetUsers();
        //private static List<ApplicationUser> GetUsers()
        //{
        //    static List<IdentityRole> GetRoles()
        //    {
        //        List<IdentityRole> list = new();
        //        // Число ролей в списке.
        //        int nRole = random.Next(2, roles.Count);
        //        IdentityRole role;
        //        // Помещаем в список list разные случайно выбранные роли из полного списка roles в количестве nRole.
        //        for (int i = 0; i < nRole; i++)
        //        {
        //            do
        //            {
        //                role = roles[random.Next(roles.Count)];
        //            } while (list.Contains(role));
        //            list.Add(role);
        //        }
        //        return list;
        //    }
        //    List<ApplicationUser> users = GetRandomUsers(random.Next(3, 10));
        //    foreach (ApplicationUser user in users)
        //    {
        //        List<IdentityRole> roles = GetRoles();
        //        List<IdentityUserRole<string>> userRoles = new();
        //        foreach (IdentityRole role in roles)
        //        {
        //            userRoles.Add(new IdentityUserRole<string>() { UserId = user.Id, RoleId = role.Id });
        //        }
        //    }
        //    return users;
        //}
        //private static List<IdentityUserRole<string>> GetUserRoles()
        //{
        //    List<IdentityUserRole<string>> userRoles = new();
        //    foreach (ApplicationUser user in users)
        //    {
        //        userRoles.Add(new IdentityUserRole<string> { RoleId = roles[random.Next(roles.Count)].Id, UserId = user.Id });
        //    }
        //    return userRoles;
        //}
        //private static string GetRandomClaim(out string? value)
        //{
        //    FieldInfo[] claimTypesFields = typeof(ClaimTypes).GetFields();
        //    FieldInfo rndField = claimTypesFields[random.Next(claimTypesFields.Length)];
        //    value = rndField.GetValue(rndField)?.ToString();
        //    return rndField.Name;
        //}
        //private static List<IdentityUserClaim<string>> GetRandomUserClaims()
        //{
        //    List<IdentityUserClaim<string>> userClaims = new();
        //    foreach (ApplicationUser applicationUser in users)
        //        userClaims.Add(new IdentityUserClaim<string> { ClaimType = GetRandomClaim(out string? value), ClaimValue = value, UserId = applicationUser.Id });
        //    return userClaims;
        //}
        //private static List<IdentityRoleClaim<string>> GetRandomRoleClaims()
        //{
        //    List<IdentityRoleClaim<string>> roleClaims = new();
        //    foreach (var role in roles)
        //    {
        //        roleClaims.Add(new IdentityRoleClaim<string> { RoleId = role.Id, ClaimType = GetRandomClaim(out string? value), ClaimValue = value });
        //    }
        //    return roleClaims;
        //}
        //private static List<IdentityUserLogin<string>> GetRandomIdentityUserLogins()
        //{
        //    List<IdentityUserLogin<string>> identityUserLogins = new();
        //    foreach (var user in users)
        //    {
        //        identityUserLogins.Add(new IdentityUserLogin<string>
        //        {
        //            UserId = user.Id,
        //            LoginProvider = GetRandomString(4),
        //            ProviderDisplayName = GetRandomString(5),
        //            ProviderKey = Guid.NewGuid().ToString()
        //        });
        //    }
        //    return identityUserLogins;
        //}
        //private static List<IdentityUserToken<string>> GetRandomIdentityUserTokens()
        //{
        //    List<IdentityUserToken<string>> identityUserTokens = new();
        //    foreach (ApplicationUser applicationUser in users)
        //    {
        //        identityUserTokens.Add(new()
        //        {
        //            UserId = applicationUser.Id,
        //            Name = GetRandomString(4),
        //            Value = GetRandomString(5),
        //            LoginProvider = GetRandomString(4)
        //        });
        //    }
        //    return identityUserTokens;
        //}
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new DataContext(serviceProvider.GetRequiredService<DbContextOptions<DataContext>>());
            if (context == null) return;
            // Чистим таблицы.
            context.Employees.ClearSave(context);
            context.Ingredients.ClearSave(context);
            context.MenuItems.ClearSave(context);
            context.Menus.ClearSave(context);
            context.OrderItems.ClearSave(context);
            context.Orders.ClearSave(context);
            context.Deliveries.ClearSave(context);
            context.Discounts.ClearSave(context);
            context.Products.ClearSave(context);
            // Заполняем таблицы случайными полями.
            context.Ingredients.AddRange(ingredients);
            context.Products.AddRange(products);
            context.OrderItems.AddRange(orderItems);
            context.MenuItems.AddRange(menuItems);
            context.Menus.AddRange(menus);
            context.Deliveries.AddRange(deliveries);
            context.Discounts.AddRange(discounts);
            context.Orders.AddRange(orders);
            context.Employees.AddRange(GetRandomEmployees(random.Next(3, 7)));
            // Сохраняем таблицы в базе.
            context.SaveChanges();
            context.MenuItems.RemoveRange(context.MenuItems.Where(mi => mi.Product == null));
            context.SaveChanges();
            context.Menus.RemoveRange(context.Menus.Where(m => m.MenuItems.Count == 0));
            context.SaveChanges();
        }

    }
}
