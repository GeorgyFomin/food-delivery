using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;

namespace WebASP_MVC.Controllers
{
    public class ProductsController : Controller
    {
        static readonly string apiAddress = "https://localhost:7234/";//Или http://localhost:5234/
        private readonly RestClient restClient = new(apiAddress);
        private static readonly string path = "api/Products";

        // GET: Products
        public async Task<IActionResult> Index()
        {
            List<Entities.Product>? products = new();
            // Одня из версий кода
            IRestResponse response = restClient.Get(new RestRequest(path));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                products = JsonConvert.DeserializeObject<List<Entities.Product>>(response.Content);
            }
            // Или
            //HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            //HttpResponseMessage response = await client.GetAsync(path);
            //if (response.IsSuccessStatusCode)
            //{
            //    var result = response.Content.ReadAsStringAsync().Result;
            //    deliveries = JsonConvert.DeserializeObject<List<Entities.Product>>(result);
            //}
            return View(products); //await _context.Product.ToListAsync());
        }
        IActionResult GetResultById(int? id)
        {
            Entities.Product? GetProduct()
            {
                IRestResponse response = restClient.Get(new RestRequest(path + $"/{id}"));
                return response.StatusCode != System.Net.HttpStatusCode.OK ? null : JsonConvert.DeserializeObject<Entities.Product>(response.Content);
            }
            Entities.Product? product;
            return id == null || (product = GetProduct()) == null ? NotFound() : View(product);
        }
        // GET: Products/Products/5
        public async Task<IActionResult> Details(int? id) => GetResultById(id);
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
        public async Task<IActionResult> Create([Bind("Name,Price,Weight,Id")] Entities.Product product)
        {
            if (ModelState.IsValid)
            {
                IRestRequest request = new RestRequest(path, Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };
                request.AddParameter("application/json; charset=utf-8", JsonConvert.SerializeObject(product), ParameterType.RequestBody);
                //IRestResponse response = 
                restClient.Execute(request);
                // Или
                //HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
                //HttpResponseMessage response = await client.PostAsJsonAsync(path, product);
                //response.EnsureSuccessStatusCode();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id) => GetResultById(id);

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Price,Weight,Id")] Entities.Product product)
        {
            if (id != product.Id)
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
                    request.AddParameter("application/json; charset=utf-8", JsonConvert.SerializeObject(product), ParameterType.RequestBody);
                    //IRestResponse response = 
                    restClient.Execute(request);
                    // Или
                    //HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
                    //HttpResponseMessage response = await client.PutAsJsonAsync(path+$"/{id}", product);
                    //response.EnsureSuccessStatusCode();

                    // Old
                    //_context.Update(product);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id) => GetResultById(id);
        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Old
            //var product = await _context.Product.FindAsync(id);
            //_context.Product.Remove(product);
            //await _context.SaveChangesAsync();

            IRestResponse response = restClient.Delete(new RestRequest(path + $"/{id}"));
            // Или
            //HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            //HttpResponseMessage response = await client.DeleteAsync(path+$"/{id}");
            //response.EnsureSuccessStatusCode();
            return RedirectToAction(nameof(Index));
        }

        private static bool ProductExists(int id)
        {
            //return _context.Product.Any(e => e.Id == id);
            return false;
        }
    }
}
