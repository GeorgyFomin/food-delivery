using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UseCases.API.Dto;

namespace WebASP_MVC.Controllers
{
    public class OrdersController : Controller
    {
        static readonly string apiAddress = "https://localhost:7234/";//Или http://localhost:5234/
        private static readonly string ordersPath = "api/Orders";
        private static readonly string productsPath = "api/Products";
        private static readonly string orderItemsPath = "api/OrderItems";
        private static OrderDto? curOrder;
        private static readonly Random random = new();
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

        public async Task<IActionResult> AddAsync(int? id)
        {
            if (id == null || curOrder == null) return NotFound();
            // Определяем продукт, из которого готовим новый элемент заказа.
            ProductDto? product = NonIncomingProducts.SingleOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            // Готовим новый элемент заказа с продуктом в количестве 1.
            OrderItemDto orderItemDto = new() { Product = product, Quantity = 1 };
            //OrderItemDto? orderItemDto = NonIncomingOrderItems.SingleOrDefault(i => i.Id == id);
            //if (orderItemDto == null) return NotFound();
            // Добавляем в список элементов заказа.
            curOrder.OrderElements.Add(orderItemDto);
            // Создаем клиента для посылки сообщений по адресу службы, обрабатывающей сообщения.
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            // Обновляем текущий заказ.
            HttpResponseMessage? response = await client.PutAsJsonAsync(ordersPath + $"/{curOrder.Id}", curOrder);
            response.EnsureSuccessStatusCode();
            // Запрос формирует обновленный список заказов. У добавленного заказа появляется Id.
            // Посылаем клиенту запрос о заказах.
            response = await client.GetAsync(ordersPath);
            //Возвращаем полученый из базы данных список заказов либо null.
            List<OrderDto>? orderDtos = response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<List<OrderDto>>(response.Content.ReadAsStringAsync().Result) : null;
            if (orderDtos == null)
                return NotFound();
            // Определяем выделенный элемент в обновленной версии списка заказов.
            curOrder = orderDtos.Find(o => o.Id == curOrder.Id);
            if (curOrder == null) return NotFound();
            // Обновляем список продуктов, удаляя из него использованный.
            //NonIncomingOrderItems.Remove(orderItemDto);
            NonIncomingProducts.Remove(product);
            // Возвращаемся к обновленной версии страницы редактирования.
            return View(curOrder);
        }
        public async Task<IActionResult> RemoveAsync(int? id)
        {
            if (id == null || curOrder == null) return NotFound();
            OrderItemDto? orderItem = curOrder.OrderElements.FirstOrDefault(i => i.Id == id);
            if (orderItem == null) return NotFound();
            // Создаем клиента для посылки сообщений по адресу службы, обрабатывающей сообщения.
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            // Удаляем элемент заказа из базы.
            HttpResponseMessage? response = await client.DeleteAsync(orderItemsPath + $"/{orderItem.Id}");
            response.EnsureSuccessStatusCode();
            curOrder.OrderElements.Remove(orderItem);
            if (orderItem.Product != null)
            {
                NonIncomingProducts.Add(orderItem.Product);
            }
            return View(curOrder);
        }
        private static async Task SaveOrderChange(OrderDto orderDto)
        {
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.PutAsJsonAsync(ordersPath + $"/{orderDto.Id}", orderDto);
            response.EnsureSuccessStatusCode();
        }
        // GET: Orders
        public async Task<IActionResult> Index()
        {
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(ordersPath);
            if (response.IsSuccessStatusCode)
            {
                List<OrderDto>? orderDtos = JsonConvert.DeserializeObject<List<OrderDto>>(response.Content.ReadAsStringAsync().Result);
                return View(orderDtos);
            }
            return NotFound();
        }
        async Task<IActionResult> GetOrderById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(ordersPath + $"/{id}");
            curOrder = null;
            if (response.IsSuccessStatusCode)
            {
                curOrder = JsonConvert.DeserializeObject<OrderDto>(response.Content.ReadAsStringAsync().Result);
            }
            if (curOrder == null)
                return NotFound();
            List<ProductDto>? productDtos = await GetProductsAsync();
            NonIncomingProducts = new();
            if (productDtos != null)
                foreach (ProductDto productDto in productDtos)
                    if (curOrder.OrderElements.FirstOrDefault(i => i.Product != null && i.Product.Id == productDto.Id) == null)
                        NonIncomingProducts.Add(productDto);
            return View(curOrder);
        }

        private static async Task<List<OrderItemDto>?> GetOrderItemsAsync()
        {
            List<OrderItemDto>? orderItemDtos = new();
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(orderItemsPath);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                orderItemDtos = JsonConvert.DeserializeObject<List<OrderItemDto>>(result);
            }
            return orderItemDtos;
        }

        // GET: Orders/Details/id
        public async Task<IActionResult> Details(int? id) => await GetOrderById(id);
        // GET: Orders/Create
        public IActionResult Create()
        {
            return View();
        }
        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Discount, Delivery, PhoneNumber, OrderItems, Id")] OrderDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return View(orderDto);
            }
            orderDto.OrderElements = new List<OrderItemDto>
            {
                new OrderItemDto()
            };
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.PostAsJsonAsync(ordersPath, orderDto);
            response.EnsureSuccessStatusCode();
            return RedirectToAction(nameof(Index));
        }

        // GET: Orders/Edit/id
        //public async Task<IActionResult> Edit(int? id, int? itemId, uint? quantity) =>
        //    id == null ? NotFound() : itemId == null ? await GetOrderById(id) : itemId < 0 ? await RemoveAsync(-itemId) :
        //    quantity == null ?
        //    await AddAsync(itemId) : await EditQuantity(itemId, quantity);

        public async Task<IActionResult> Edit(int? id, int? itemId) =>
            id == null ? NotFound() : itemId == null ? await GetOrderById(id) : itemId < 0 ? await RemoveAsync(-itemId) :
            await AddAsync(itemId);
        //private async Task<IActionResult> EditQuantity(int? itemId, uint? quantity)
        //{
        //    if (itemId == null || curOrder == null || quantity == null)
        //    {
        //        return NotFound();
        //    }
        //    OrderItemDto? orderItemDto = curOrder.OrderElements.FirstOrDefault(curOrder => curOrder.Id == itemId);
        //    if (orderItemDto == null)
        //    {
        //        return NotFound();
        //    }
        //    orderItemDto.Quantity = quantity.Value;
        //    HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
        //    HttpResponseMessage response = await client.PutAsJsonAsync(orderItemsPath + $"/{orderItemDto.Id}", orderItemDto);
        //    response.EnsureSuccessStatusCode();
        //    return RedirectToAction(nameof(Index));
        //}

        // POST: Orders/Edit/id
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Discount, Delivery, PhoneNumber, Id")] OrderDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return View(orderDto);
            }
            if (curOrder != null)
            {
                orderDto.OrderElements = curOrder.OrderElements;
            }
            try
            {
                await SaveOrderChange(orderDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (GetOrderById(orderDto.Id).IsFaulted)
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

        // GET: Orders/Delete/id
        public async Task<IActionResult> Delete(int? id) => await GetOrderById(id);
        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.DeleteAsync(ordersPath + $"/{id}");
            response.EnsureSuccessStatusCode();
            return RedirectToAction(nameof(Index));
        }
    }
}
