using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UseCases.API.Dto;

namespace WebASP_MVC.Controllers
{
    public class OrderItemsController : Controller
    {
        static readonly string apiAddress = "https://localhost:7234/";//Или http://localhost:5234/
        private static readonly string orderItemsPath = "api/OrderItems";
        private static readonly string path = "api/Products";

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
        // GET: OrderItems
        public async Task<IActionResult> Index()
        {
            List<OrderItemDto>? orderItemDtos = new();
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(orderItemsPath);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                orderItemDtos = JsonConvert.DeserializeObject<List<OrderItemDto>>(result);
            }
            return View(orderItemDtos);
        }
        static int? productId;
        async Task<IActionResult> GetOrderItemById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            OrderItemDto? orderItemDto = null;
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(orderItemsPath + $"/{id}");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                orderItemDto = JsonConvert.DeserializeObject<OrderItemDto>(result);
            }
            productId = orderItemDto?.Product?.Id;

            return orderItemDto == null ? NotFound() : View(orderItemDto);
        }
        // GET: OrderItems/Details/id
        public async Task<IActionResult> Details(int? id) => await GetOrderItemById(id);
        // GET: OrderItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OrderItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Product, Quantity, Id")] OrderItemDto orderItemDto)
        {
            if (!ModelState.IsValid)
            {
                return View(orderItemDto);
            }
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.PostAsJsonAsync(orderItemsPath, orderItemDto);
            response.EnsureSuccessStatusCode();
            return RedirectToAction(nameof(Index));
        }

        // GET: OrderItems/Edit/id
        public async Task<IActionResult> Edit(int? id) => await GetOrderItemById(id);

        // POST: OrderItems/Edit/id
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Product, Quantity, Id")] OrderItemDto orderItemDto)
        {
            if (!ModelState.IsValid)
            {
                return View(orderItemDto);
            }
            if (productId != null)
            {
                List<ProductDto>? products = await GetProductsAsync();
                if (products != null)
                    orderItemDto.Product = products.FirstOrDefault(p => p.Id == productId);
            }
            try
            {
                HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
                HttpResponseMessage response = await client.PutAsJsonAsync(orderItemsPath + $"/{orderItemDto.Id}", orderItemDto);
                response.EnsureSuccessStatusCode();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (GetOrderItemById(orderItemDto.Id).IsFaulted)
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

        // GET: OrderItems/Delete/id
        public async Task<IActionResult> Delete(int? id) => await GetOrderItemById(id);
        // POST: OrderItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.DeleteAsync(orderItemsPath + $"/{id}");
            response.EnsureSuccessStatusCode();
            return RedirectToAction(nameof(Index));
        }
    }
}
