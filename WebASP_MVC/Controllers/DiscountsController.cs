using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UseCases.API.Dto;

namespace WebASP_MVC.Controllers
{
    public class DiscountsController : Controller
    {
        static readonly string apiAddress = "https://localhost:7234/";//Или http://localhost:5234/
        private static readonly string path = "api/Discounts";

        // GET: Deliveries
        public async Task<IActionResult> Index()
        {
            List<DiscountDto>? discountDtos = new();
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                discountDtos = JsonConvert.DeserializeObject<List<DiscountDto>>(result);
            }
            return View(discountDtos);
        }
        async Task<IActionResult> GetDiscountById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            DiscountDto? discountDto = null;
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(path + $"/{id}");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                discountDto = JsonConvert.DeserializeObject<DiscountDto>(result);
            }
            return discountDto == null ? NotFound() : View(discountDto);
        }
        // GET: Deliveries/Details/id
        public async Task<IActionResult> Details(int? id) => await GetDiscountById(id);
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
        public async Task<IActionResult> Create([Bind("Type,Size,Id")] DiscountDto discountDto)
        {
            if (!ModelState.IsValid)
            {
                return View(discountDto);
            }
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.PostAsJsonAsync(path, discountDto);
            response.EnsureSuccessStatusCode();
            return RedirectToAction(nameof(Index));
        }

        // GET: Deliveries/Edit/id
        public async Task<IActionResult> Edit(int? id) => await GetDiscountById(id);

        // POST: Deliveries/Edit/id
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Type,Size,Id")] DiscountDto discountDto)
        {
            if (!ModelState.IsValid)
            {
                return View(discountDto);
            }
            try
            {
                HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
                HttpResponseMessage response = await client.PutAsJsonAsync(path + $"/{discountDto.Id}", discountDto);
                response.EnsureSuccessStatusCode();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (GetDiscountById(discountDto.Id).IsFaulted)
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

        // GET: Deliveries/Delete/id
        public async Task<IActionResult> Delete(int? id) => await GetDiscountById(id);
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
