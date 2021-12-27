using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Entities.Domain;

namespace WebASP_MVC.Controllers
{
    public class DeliveriesController : Controller
    {
        static readonly string apiAddress = "https://localhost:7234/";//Или http://localhost:5234/
        private static readonly string path = "api/Deliveries";

        // GET: Deliveries
        public async Task<IActionResult> Index()
        {
            List<Delivery>? deliveries = new();
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                deliveries = JsonConvert.DeserializeObject<List<Delivery>>(result);
            }
            return View(deliveries);
        }
        async Task<IActionResult> GetDeliveryById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Delivery? delivery = null;
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(path + $"/{id}");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                delivery = JsonConvert.DeserializeObject<Delivery>(result);
            }
            return delivery == null ? NotFound() : View(delivery);
        }
        // GET: Deliveries/Details/5
        public async Task<IActionResult> Details(int? id) => await GetDeliveryById(id);
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
        public async Task<IActionResult> Create([Bind("ServiceName,Price,TimeSpan,Id")] Delivery delivery)
        {
            if (!ModelState.IsValid)
            {
                return View(delivery);
            }
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.PostAsJsonAsync(path, delivery);
            response.EnsureSuccessStatusCode();
            return RedirectToAction(nameof(Index));
        }

        // GET: Deliveries/Edit/5
        public async Task<IActionResult> Edit(int? id) => await GetDeliveryById(id);

        // POST: Deliveries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("ServiceName,Price,TimeSpan,Id")] Delivery delivery)
        {
            if (!ModelState.IsValid)
            {
                return View(delivery);
            }
            try
            {
                HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
                HttpResponseMessage response = await client.PutAsJsonAsync(path, delivery);
                response.EnsureSuccessStatusCode();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (GetDeliveryById(delivery.Id).IsFaulted)
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

        // GET: Deliveries/Delete/5
        public async Task<IActionResult> Delete(int? id) => await GetDeliveryById(id);
        // POST: Deliveries/Delete/5
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
