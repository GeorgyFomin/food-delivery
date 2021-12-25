using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


namespace WebASP_MVC.Controllers
{
    public class IngredientsController : Controller
    {
        static readonly string apiAddress = "https://localhost:7234/";//Или http://localhost:5234/
        private static readonly string path = "api/Ingredients";

        // GET: Ingredients
        public async Task<IActionResult> Index()
        {
            List<Ingredient>? ingredients = new();
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                ingredients = JsonConvert.DeserializeObject<List<Entities.Ingredient>>(result);
            }
            return View(ingredients);
        }
        async Task<IActionResult> GetIngredientById(int? id)
        {
            if (id == null)
                return NotFound();
            Ingredient? ingredient = null;
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(path + $"/{id}");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                ingredient = JsonConvert.DeserializeObject<Entities.Ingredient>(result);
            }
            return ingredient == null ? NotFound() : View(ingredient);
        }
        // GET: Ingredients/Details/5
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
        public async Task<IActionResult> Create([Bind("Name")] Ingredient ingredient)
        {
            if (!ModelState.IsValid)
            {
                return View(ingredient);
            }
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.PostAsJsonAsync(path, ingredient);
            response.EnsureSuccessStatusCode();
            return RedirectToAction(nameof(Index));
        }

        // GET: Ingredients/Edit/5
        public async Task<IActionResult> Edit(int? id) => await GetIngredientById(id);

        // POST: Ingredients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Name,Id")] Ingredient ingredient)
        {
            if (!ModelState.IsValid)
            {
                return View(ingredient);
            }
            try
            {
                HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
                HttpResponseMessage response = await client.PutAsJsonAsync(path, ingredient);// + $"/{id}"
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

        // GET: Ingredients/Delete/5
        public async Task<IActionResult> Delete(int? id) => await GetIngredientById(id);
        // POST: Ingredients/Delete/5
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
