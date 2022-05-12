using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UseCases.API.Dto;

namespace WebASP_MVC.Controllers
{
    public class MenusController : Controller
    {
        static readonly string apiAddress = "https://localhost:7234/";//Или http://localhost:5234/
        private static readonly string menusPath = "api/Menus";
        private static readonly string itemsPath = "api/MenuItems";
        private static readonly string productsPath = "api/Products";
        static MenuDto? curMenu = null;
        public static List<ProductDto> IncomingProducts { private set; get; } = new List<ProductDto>();
        public static List<ProductDto> NonIncomingProducts { private set; get; } = new List<ProductDto>();
        static async Task<List<ProductDto>?> GetProductsAsync()
        {
            List<ProductDto>? products = new();
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(productsPath);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                products = JsonConvert.DeserializeObject<List<ProductDto>>(result);
            }
            return products;
        }
        public IActionResult Add(int? id)
        {
            if (id == null || curMenu == null) return NotFound();
            ProductDto? product = NonIncomingProducts.SingleOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            MenuItemDto menuItemDto = new() { Product = product };
            IncomingProducts.Add(product);
            curMenu.MenuItems.Add(menuItemDto);
            NonIncomingProducts.Remove(product);
            return View(curMenu);
        }
        public async Task<IActionResult> RemoveAsync(int? id)
        {
            if (id == null || curMenu == null) return NotFound();
            ProductDto? product = IncomingProducts.SingleOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            MenuItemDto? menuItemDto = curMenu.MenuItems.Find(i => i.Product != null && i.Product.Id == id);
            if (menuItemDto == null) return NotFound();
            // Посылаем запрос на редактирование меню.
            // Создаем клиента для посылки сообщений по адресу службы, обрабатывающей сообщения.
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            // Посылаем запрос на удаление элемента меню.
            HttpResponseMessage? response = await client.DeleteAsync(itemsPath + $"/{menuItemDto.Id}");
            response.EnsureSuccessStatusCode();
            //curMenu.MenuItems.Remove(menuItemDto);
            IncomingProducts.Remove(product);
            NonIncomingProducts.Add(product);
            return View(curMenu);
        }
        private static async Task SaveMenuChange(MenuDto menuDto)
        {
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.PutAsJsonAsync(menusPath + $"/{menuDto.Id}", menuDto);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IActionResult> Index()
        {
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(menusPath);
            if (response.IsSuccessStatusCode)
            {
                List<MenuDto>? menuDtos = JsonConvert.DeserializeObject<List<MenuDto>>(response.Content.ReadAsStringAsync().Result);
                return View(menuDtos);
            }
            return NotFound();
        }
        async Task<IActionResult> GetMenuById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(menusPath + $"/{id}");
            curMenu = null;
            if (response.IsSuccessStatusCode)
            {
                curMenu = JsonConvert.DeserializeObject<MenuDto>(response.Content.ReadAsStringAsync().Result);
            }
            if (curMenu == null)
                return NotFound();
            IncomingProducts = new();
            foreach (MenuItemDto menuItemDto in curMenu.MenuItems)
            {
                if (menuItemDto.Product == null) continue;
                IncomingProducts.Add(menuItemDto.Product);
            }
            List<ProductDto>? productDtos = await GetProductsAsync();
            NonIncomingProducts = new();
            if (productDtos != null)
                foreach (ProductDto productDto in productDtos)
                    if (IncomingProducts.Find(p => p.Id == productDto.Id) == null)
                        NonIncomingProducts.Add(productDto);
            return View(curMenu);
        }
        // GET: Menus/Details/id
        public async Task<IActionResult> Details(int? id) => await GetMenuById(id);
        // GET: Menus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Deliveries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MenuItems,Id")] MenuDto menuDto)
        {
            if (!ModelState.IsValid)
            {
                return View(menuDto);
            }
            List<ProductDto>? products = await GetProductsAsync();
            if (products == null)
                return BadRequest();
            menuDto.MenuItems = new List<MenuItemDto>();
            // Заполняем его элементы меню всеми продуктами из базы.
            foreach (ProductDto productDto in products)
            {
                // Добавляем к элементам меню ссылку на продукт.
                menuDto.MenuItems.Add(new MenuItemDto { Product = productDto });
            }
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.PostAsJsonAsync(menusPath, menuDto);
            response.EnsureSuccessStatusCode();
            return RedirectToAction(nameof(Index));
        }

        // GET: Menus/Edit/id
        public async Task<IActionResult> Edit(int? id, int? itemId) =>
            id == null ? NotFound() : itemId == null ? await GetMenuById(id) : itemId < 0 ? await RemoveAsync(-itemId) : Add(itemId);

        // POST: Menus/Edit/id
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("MenuItems,Id")] MenuDto menuDto)
        {
            if (!ModelState.IsValid)
            {
                return View(menuDto);
            }
            try
            {
                if (curMenu != null)
                {
                    menuDto.MenuItems = curMenu.MenuItems;
                    menuDto.Id = curMenu.Id;
                }
                await SaveMenuChange(menuDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (GetMenuById(menuDto.Id).IsFaulted)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Menus/Delete/id
        public async Task<IActionResult> Delete(int? id) => await GetMenuById(id);
        // POST: Menus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.DeleteAsync(menusPath + $"/{id}");
            response.EnsureSuccessStatusCode();
            return RedirectToAction(nameof(Index));
        }
    }
}
