using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UseCases.API.Dto;

namespace WebASP_MVC.Controllers
{
    public class OrdersController : Controller
    {
        static readonly string apiAddress = "https://localhost:7234/";//Или http://localhost:5234/
        private static readonly string discountsPath = "api/Discounts";
        private static readonly string deliveriesPath = "api/Deliveries";
        private static readonly string itemsPath = "api/OrderItems";
        private static readonly string ordersPath = "api/Orders";
        private static readonly string productsPath = "api/Products";
        private static OrderDto? curOrder;
        private static readonly Random random = new();
        public static List<OrderItemDto> IncomingItems { set; get; } = new List<OrderItemDto>();
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
        public IActionResult Add(int? id)
        {
            if (id == null || curOrder == null) return NotFound();
            ProductDto? product = NonIncomingProducts.SingleOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            OrderItemDto orderItemDto = new() { Product = product, Quantity = 1 };
            IncomingItems.Add(orderItemDto);
            curOrder.OrderElements.Add(orderItemDto);
            NonIncomingProducts.Remove(product);
            return View(curOrder);
        }
        public async Task<IActionResult> RemoveAsync(int? id)
        {
            if (id == null || curOrder == null) return NotFound();
            OrderItemDto? orderItem = IncomingItems.FirstOrDefault(i => i.Id == id);
            if (orderItem == null) return NotFound();
            // Создаем клиента для посылки сообщений по адресу службы, обрабатывающей сообщения.
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            // Удаляем элемент заказа из базы.
            HttpResponseMessage? response = await client.DeleteAsync(itemsPath + $"/{orderItem.Id}");
            response.EnsureSuccessStatusCode();
            IncomingItems.Remove(orderItem);
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
            IncomingItems = new List<OrderItemDto>(curOrder.OrderElements);
            List<ProductDto>? productDtos = await GetProductsAsync();
            NonIncomingProducts = new();
            if (productDtos != null)
                foreach (ProductDto productDto in productDtos)
                    if (IncomingItems.Find(i => i.Product != null && i.Product.Id == productDto.Id) == null)
                        NonIncomingProducts.Add(productDto);
            return View(curOrder);
        }
        // GET: Orders/Details/id
        public async Task<IActionResult> Details(int? id) => await GetOrderById(id);
        // GET: Orders/Create
        public IActionResult Create()
        {
            return View();
        }
        private async Task<DeliveryDto?> GetRandomDelivery()
        {
            List<DeliveryDto>? deliveryDtos = new();
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(deliveriesPath);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                deliveryDtos = JsonConvert.DeserializeObject<List<DeliveryDto>>(result);
            }
            return deliveryDtos?[random.Next(deliveryDtos.Count)];
        }
        private async Task<DiscountDto?> GetRandomDiscount()
        {
            List<DiscountDto>? discountDtos = new();
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(discountsPath);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                discountDtos = JsonConvert.DeserializeObject<List<DiscountDto>>(result);
            }
            return discountDtos?[random.Next(discountDtos.Count)];
        }
        private static async Task<List<OrderItemDto>?> GetRandomOrderItems()
        {
            List<OrderItemDto>? orderItemDtos = new();
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(itemsPath);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                orderItemDtos = JsonConvert.DeserializeObject<List<OrderItemDto>>(result);
            }
            //if (orderItemDtos == null || orderItemDtos.Count == 0)
            //{
            //    return new List<OrderItemDto>() { new OrderItemDto() { Product = Products[0], Quantity = 1 } };
            //}
            //int amount = random.Next(1, 9);
            //while (orderItemDtos.Count > amount)
            //    // Убираем случайный элемент из всего списка элементов заказов, если в списке больше одного элемента. 
            //    orderItemDtos.Remove(orderItemDtos[random.Next(orderItemDtos.Count)]);
            return orderItemDtos;
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Discount, Delivery, PhoneNumber, OrderItems,Id")] OrderDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return View(orderDto);
            }
            orderDto.Discount = await GetRandomDiscount();
            orderDto.Delivery = await GetRandomDelivery();
            List<OrderItemDto>? orderItemDtos = await GetRandomOrderItems();
            if (orderItemDtos != null)
                orderDto.OrderElements = orderItemDtos;
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.PostAsJsonAsync(ordersPath, orderDto);
            response.EnsureSuccessStatusCode();
            return RedirectToAction(nameof(Index));
        }

        // GET: Orders/Edit/id
        public async Task<IActionResult> Edit(int? id, int? itemId) =>
            id == null ? NotFound() : itemId == null ? await GetOrderById(id) : itemId < 0 ? await RemoveAsync(-itemId) : Add(itemId);

        // POST: Orders/Edit/id
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Discount, Delivery, PhoneNumber, OrderItems,Id")] OrderDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return View(orderDto);
            }
            try
            {
                orderDto.OrderElements = IncomingItems;
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
