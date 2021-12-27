using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Entities.Domain;

namespace WebASP_MVC.Controllers
{
    public class ProductsController : Controller
    {
        static readonly string apiAddress = "https://localhost:7234/";//Или http://localhost:5234/
        private static readonly string path = "api/Products";

        // GET: Products
        public async Task<IActionResult> Index()
        {
            List<Product>? products = new();
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                products = JsonConvert.DeserializeObject<List<Product>>(result);
            }
            return View(products);
        }
        async Task<IActionResult> GetProductById(int? id)
        {
            if (id == null)
                return NotFound();
            Product? product = null;
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(path + $"/{id}");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                product = JsonConvert.DeserializeObject<Product>(result);
            }
            return product == null ? NotFound() : View(product);
        }
        // GET: Products/Products/5
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
        public async Task<IActionResult> Create([Bind("Name,Price,Weight,Ingredients")] Product product)
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

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id) => await GetProductById(id);

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Name,Price,Weight,Ingredients,Id")] Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            try
            {
                HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
                HttpResponseMessage response = await client.PutAsJsonAsync(path, product);
                response.EnsureSuccessStatusCode();
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

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id) => await GetProductById(id);
        // POST: Products/Delete/5
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
