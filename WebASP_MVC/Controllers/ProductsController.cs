using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Entities.Domain;
using UseCases.API.Dto;

namespace WebASP_MVC.Controllers
{
    public class ProductsController : Controller
    {
        public const int idLmt = 1_000_000_000;
        static readonly string apiAddress = "https://localhost:7234/";//Или http://localhost:5234/
        private static readonly string path = "api/Products";
        static ProductDto? curProduct = null;
        private static async Task<List<IngredientDto>?> GetIngredientsAsync()
        {
            List<IngredientDto>? ingredients = new();
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync("api/Ingredients");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                ingredients = JsonConvert.DeserializeObject<List<IngredientDto>>(result);
            }
            return ingredients;
        }
        static async Task<List<ProductDto>?> GetProductsAsync()
        {
            List<ProductDto>? products = new();
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                products = JsonConvert.DeserializeObject<List<ProductDto>>(result);
            }
            return products;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await GetProductsAsync());
        }
        public static List<IngredientDto> IncomingIngredients { private set; get; } = new List<IngredientDto>();
        public static List<IngredientDto> NonIncomingIngredients { get; set; } = new List<IngredientDto>();
        public IActionResult Add(int? id)
        {
            if (id == null || curProduct == null)
            {
                return NotFound();
            }
            IngredientDto? ingredientDto = NonIncomingIngredients.SingleOrDefault(i => i.Id == id);
            if (ingredientDto == null)
            {
                return NotFound();
            }
            IncomingIngredients.Add(ingredientDto);
            curProduct.ProductsIngredients.Add(new ProductIngredientDto() { IngredientId = ingredientDto.Id, ProductId = curProduct.Id });
            NonIncomingIngredients.Remove(ingredientDto);
            return View(curProduct);
        }
        public IActionResult Remove(int? id)
        {
            if (id == null || curProduct == null)
            {
                return NotFound();
            }
            IngredientDto? ingredientDto = IncomingIngredients.SingleOrDefault(i => i.Id == id);
            if (ingredientDto == null)
            {
                return NotFound();
            }
            IncomingIngredients.Remove(ingredientDto);
            ProductIngredientDto? productIngredient = curProduct.ProductsIngredients.FirstOrDefault(pi => pi.IngredientId == ingredientDto.Id);
            if (productIngredient != null)
            {
                curProduct.ProductsIngredients.Remove(productIngredient);
            }
            NonIncomingIngredients.Add(ingredientDto);
            return View(curProduct);
        }
        private static async Task SaveProductChange(ProductDto productDto)
        {
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.PutAsJsonAsync(path + $"/{productDto.Id}", productDto);
            response.EnsureSuccessStatusCode();
        }
        async Task<IActionResult> GetProductById(int? id)
        {
            if (id == null)
                return NotFound();
            curProduct = null;
            List<ProductDto>? products = await GetProductsAsync();
            if (products == null)
                return NotFound();
            curProduct = products.SingleOrDefault(p => p.Id == id);
            if (curProduct == null)
            {
                return NotFound();
            }
            List<IngredientDto>? ingredients = await GetIngredientsAsync();
            if (ingredients != null)
            {
                IncomingIngredients = new();
                foreach (ProductIngredientDto productIngredient in curProduct.ProductsIngredients)
                {
                    IngredientDto? ingredientDto = ingredients.Find(i => i.Id == productIngredient.IngredientId);
                    if (ingredientDto != null)
                    {
                        IncomingIngredients.Add(ingredientDto);
                    }
                }
                NonIncomingIngredients = new();
                foreach (IngredientDto ingredient in ingredients)
                {
                    if (ingredient.ProductsIngredients == null || ingredient.ProductsIngredients.Find(pi => pi.ProductId == curProduct.Id) == default)
                    {
                        NonIncomingIngredients.Add(ingredient);
                    }
                }
            }
            return View(curProduct);
        }
        // GET: Products/Products/id
        public async Task<IActionResult> Details(int? id) => await GetProductById(id);
        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Price,Weight,ProductsIngredients")] ProductDto product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.PostAsJsonAsync(path, product);
            response.EnsureSuccessStatusCode();
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Edit/id
        public async Task<IActionResult> Edit(int? id, int? ingrId) =>
            id == null ? NotFound() : ingrId == null ? await GetProductById(id) : ingrId < 0 ? Remove(-ingrId) : Add(ingrId);
        // POST: Products/Edit/id
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Name,Price,Weight,ProductsIngredients,Id")] ProductDto product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            try
            {
                if (curProduct != null)
                {
                    product.ProductsIngredients = curProduct.ProductsIngredients;
                    product.Id = curProduct.Id;
                }
                await SaveProductChange(product);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (GetProductById(product.Id).IsFaulted)
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

        // GET: Products/Delete/id
        public async Task<IActionResult> Delete(int? id) => await GetProductById(id);
        // POST: Products/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await GetProductById(id);
            if (curProduct == null)
                return NotFound();
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.DeleteAsync(path + $"/{id}");
            response.EnsureSuccessStatusCode();
            return RedirectToAction(nameof(Index));
        }
    }
}
