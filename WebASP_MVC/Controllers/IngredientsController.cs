using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;

namespace WebASP_MVC.Controllers
{
    public class IngredientsController : Controller
    {
        static readonly string apiAddress = "https://localhost:7234/";//Или http://localhost:5234/
        private readonly RestClient restClient = new(apiAddress);
        private static readonly string path = "api/Ingredients";

        // GET: Deliveries
        public async Task<IActionResult> Index()
        {
            List<Entities.Ingredient>? ingredients = new();
            // Одня из версий кода
            IRestResponse response = restClient.Get(new RestRequest(path));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                ingredients = JsonConvert.DeserializeObject<List<Entities.Ingredient>>(response.Content);
            }
            // Или
            //HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            //HttpResponseMessage response = await client.GetAsync("api/Deliveries");
            //if (response.IsSuccessStatusCode)
            //{
            //    var result = response.Content.ReadAsStringAsync().Result;
            //    deliveries = JsonConvert.DeserializeObject<List<Entities.Ingredient>>(result);
            //}
            return View(ingredients); //await _context.Ingredient.ToListAsync());
        }
        IActionResult GetResultById(int? id)
        {
            Entities.Ingredient? GetIngredient()
            {
                IRestResponse response = restClient.Get(new RestRequest(path + $"/{id}"));
                return response.StatusCode != System.Net.HttpStatusCode.OK ? null : JsonConvert.DeserializeObject<Entities.Ingredient>(response.Content);
            }
            Entities.Ingredient? ingredient;
            return id == null || (ingredient = GetIngredient()) == null ? NotFound() : View(ingredient);
        }
        // GET: Deliveries/Details/5
        public async Task<IActionResult> Details(int? id) => GetResultById(id);
        // GET: Deliveries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Deliveries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Id")] Entities.Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {
                IRestRequest request = new RestRequest(path, Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };
                request.AddParameter("application/json; charset=utf-8", JsonConvert.SerializeObject(ingredient), ParameterType.RequestBody);
                //IRestResponse response = 
                restClient.Execute(request);
                // Или
                //HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
                //HttpResponseMessage response = await client.PostAsJsonAsync("api/Deliveries", ingredient);
                //response.EnsureSuccessStatusCode();
                return RedirectToAction(nameof(Index));
            }
            return View(ingredient);
        }

        // GET: Deliveries/Edit/5
        public async Task<IActionResult> Edit(int? id) => GetResultById(id);

        // POST: Deliveries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Id")] Entities.Ingredient ingredient)
        {
            if (id != ingredient.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    IRestRequest request = new RestRequest(path + $"/{id}", Method.PUT)
                    {
                        RequestFormat = DataFormat.Json
                    };
                    request.AddParameter("application/json; charset=utf-8", JsonConvert.SerializeObject(ingredient), ParameterType.RequestBody);
                    //IRestResponse response = 
                    restClient.Execute(request);
                    // Или
                    //HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
                    //HttpResponseMessage response = await client.PutAsJsonAsync($"api/Deliveries/{id}", ingredient);
                    //response.EnsureSuccessStatusCode();

                    // Old
                    //_context.Update(ingredient);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IngredientExists(ingredient.Id))
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
            return View(ingredient);
        }

        // GET: Deliveries/Delete/5
        public async Task<IActionResult> Delete(int? id) => GetResultById(id);
        // POST: Deliveries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Old
            //var Ingredient = await _context.Ingredient.FindAsync(id);
            //_context.Ingredient.Remove(Ingredient);
            //await _context.SaveChangesAsync();

            IRestResponse response = restClient.Delete(new RestRequest(path + $"/{id}"));
            // Или
            //HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            //HttpResponseMessage response = await client.DeleteAsync(path+ $"/{id}");
            //response.EnsureSuccessStatusCode();
            return RedirectToAction(nameof(Index));
        }
        private bool IngredientExists(int id)
        {
            //return _context.Ingredient.Any(e => e.Id == id);
            return false;
        }
    }
}
