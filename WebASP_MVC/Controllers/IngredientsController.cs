using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UseCases.API.Dto;

namespace WebASP_MVC.Controllers
{
    public class IngredientsController : Controller
    {
        static readonly string apiAddress = "https://localhost:7234/";//Или http://localhost:5234/
        private static readonly string path = "api/Ingredients";

        private static async Task<List<IngredientDto>?> GetIngredientsAsync()
        {
            List<IngredientDto>? ingredients = new();
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                ingredients = JsonConvert.DeserializeObject<List<IngredientDto>>(result);
            }
            return ingredients;
        }
        // GET: Ingredients
        public async Task<IActionResult> Index()
        {
            return View(await GetIngredientsAsync());
        }
        static async Task<List<ProductDto>?> GetProductsAsync()
        {
            List<ProductDto>? products = new();
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync("api/Products");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                products = JsonConvert.DeserializeObject<List<ProductDto>>(result);
            }
            return products;
        }
        public static List<ProductDto>? ProductsWithIngredient { private set; get; }
        static List<ProductIngredientDto>? productIngredientDtos = null;
        async Task<IActionResult> GetIngredientById(int? id)
        {
            if (id == null)
                return NotFound();
            IngredientDto? ingredient = null;
            List<IngredientDto>? ingredients = await GetIngredientsAsync();
            if (ingredients != null)
            {
                ingredient = ingredients.SingleOrDefault(i => i.Id == id);
                if (ingredient != null)
                {
                    List<ProductDto>? products = await GetProductsAsync();
                    if (products != null && ingredient.ProductsIngredients != null)
                    {
                        ProductsWithIngredient = new();
                        foreach (ProductIngredientDto productIngredient in ingredient.ProductsIngredients)
                        {
                            ProductDto? productDto = products.Find(p => p.Id == productIngredient.ProductId);
                            if (productDto != null)
                            {
                                ProductsWithIngredient.Add(productDto);
                            }
                        }
                    }
                }
            }
            productIngredientDtos = ingredient?.ProductsIngredients;
            return ingredient == null ? NotFound() : View(ingredient);

        }
        // GET: Ingredients/Details/id
        public async Task<IActionResult> Details(int? id) => await GetIngredientById(id);
        // GET: Ingredients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ingredients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name, ProductsIngredients")] IngredientDto ingredientDto)
        {
            if (!ModelState.IsValid)
            {
                return View(ingredientDto);
            }
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.PostAsJsonAsync(path, ingredientDto);
            response.EnsureSuccessStatusCode();
            return RedirectToAction(nameof(Index));
        }

        // GET: Ingredients/Edit/5
        public async Task<IActionResult> Edit(int? id) => await GetIngredientById(id);

        // POST: Ingredients/Edit/id
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("ProductsIngredients, Name, Id")] IngredientDto ingredient)
        {
            ingredient.ProductsIngredients = productIngredientDtos;
            if (!ModelState.IsValid)
            {
                return View(ingredient);
            }
            try
            {
                HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
                HttpResponseMessage response = await client.PutAsJsonAsync(path + $"/{ingredient.Id}", ingredient);// 
                response.EnsureSuccessStatusCode();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (GetIngredientById(ingredient.Id).IsFaulted)
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

        // GET: Ingredients/Delete/id
        public async Task<IActionResult> Delete(int? id) => await GetIngredientById(id);
        // POST: Ingredients/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.DeleteAsync(path + $"/{id}");
            response.EnsureSuccessStatusCode();
            return RedirectToAction(nameof(Index));
        }
    }
}
