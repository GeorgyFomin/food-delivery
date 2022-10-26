using Entities.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using UseCases.API.Dto;
using WpfApp.Commands;

namespace WpfApp.ViewModels
{
    internal class OrdersViewModel : ViewModelBase
    {
        #region Fields
        /// <summary>
        /// Хранит индекс последней выделенной строки в таблице заказов.
        /// </summary>
        int lastSelectedOrderRowIndx;
        /// <summary>
        /// Хранит индекс вновь образованной строки в таблице заказов.
        /// </summary>
        int newRowIndex;
        /// <summary>
        /// Хранит базовый адрес службы API, используемой для разделения запросов и команд при доступе к базе данных.
        /// </summary>
        private static readonly string apiAddress = "https://localhost:7234/";//Или http://localhost:5234/
        /// <summary>
        /// Хранит маршрут к контроллеру Orders.
        /// </summary>
        private readonly string ordersControllerPath = "api/Orders";
        /// <summary>
        /// Хранит маршрут к контроллеру OrderItems.
        /// </summary>
        private readonly string orderItemsControllerPath = "api/OrderItems";
        /// <summary>
        /// Хранит маршрут к контроллеру Products.
        /// </summary>
        private readonly string productsControllerPath = "api/Products";
        /// <summary>
        /// Хранит ссылку на коллекцию объектов модели.
        /// </summary>
        private ObservableCollection<ProductDto> products = new();
        /// <summary>
        /// Хранит ссылку на таблицу с заказами.
        /// </summary>
        private DataGrid? orderGrid;
        /// <summary>
        /// Хранит ссылку на текущий выделенный объект модели.
        /// </summary>
        private OrderItemDto? selectedOrderItem;
        /// <summary>
        /// Хранит ссылку на коллекцию объектов модели.
        /// </summary>
        private ObservableCollection<OrderItemDto> orderItems = new();
        /// <summary>
        /// Хранит ссылку на текущий выделенный объект модели.
        /// </summary>
        private OrderDto? selectedOrder;
        /// <summary>
        /// Хранит ссылку на коллекцию объектов модели.
        /// </summary>
        private ObservableCollection<OrderDto> orders = new();
        /// <summary>
        /// Хранит ссылку на команду выделения строки в таблице UI.
        /// </summary>
        private RelayCommand? orderSelectionCommand;
        /// <summary>
        /// Хранит ссылку на команду стирания записи.
        /// </summary>
        private RelayCommand? orderRemoveCommand;
        private RelayCommand? orderItemSelectionCommand;
        private RelayCommand? productSelectionCommand;
        private RelayCommand? orderRowEditEndCommand;
        private RelayCommand? orderItemRemoveCommand;
        private RelayCommand? orderItemRowEditEndCommand;
        #endregion
        #region Properties
        public OrderDto? SelectedOrder { get => selectedOrder; set { selectedOrder = value; RaisePropertyChanged(nameof(selectedOrder)); } }
        /// <summary>
        /// Устанавливает и возвращает ссылку на коллекцию продуктов.
        /// </summary>
        public ObservableCollection<ProductDto> Products
        {
            get => products;
            set { products = value; RaisePropertyChanged(nameof(Products)); }
        }
        public ObservableCollection<OrderItemDto> OrderItems { get => orderItems; set { orderItems = value; RaisePropertyChanged(nameof(OrderItems)); } }
        /// <summary>
        /// Устанавливает и возвращает коллекцию заказов.
        /// </summary>
        public ObservableCollection<OrderDto> Orders { get => orders; set { orders = value; RaisePropertyChanged(nameof(Orders)); } }
        /// <summary>
        /// Устанавливает и возвращает ссылку на команду выделения строки в таблице заказов.
        /// </summary>
        public ICommand OrderSelectionCommand => orderSelectionCommand ??= new RelayCommand(OrderSelection);
        /// <summary>
        /// Устанавливает и возвращает ссылку на команду стирания записи в таблице заказов.
        /// </summary>
        public ICommand OrderRemoveCommand => orderRemoveCommand ??= new RelayCommand(RemoveSelectedOrder);
        public ICommand OrderItemSelectionCommand => orderItemSelectionCommand ??= new RelayCommand(OrderItemSelection);
        public ICommand ProductSelectionCommand => productSelectionCommand ??= new RelayCommand(ProductSelection);
        public ICommand OrderRowEditEndCommand => orderRowEditEndCommand ??= new RelayCommand(OrderRowEditEnd);
        public ICommand OrderItemRemoveCommand => orderItemRemoveCommand ??= new RelayCommand(RemoveOrderItem);
        public ICommand OrderItemRowEditEndCommand => orderItemRowEditEndCommand ??= new RelayCommand(OrderItemRowEditEnd);
        #endregion
        public OrdersViewModel()
        {
            Upload();
        }
        async Task<List<OrderDto>?> GetOrders()
        {
            // Создаем клиента для посылки запроса по адресу службы.
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            // Посылаем клиенту запрос о заказах.
            HttpResponseMessage response = await client.GetAsync(ordersControllerPath);
            //Возвращаем полученый из базы данных список заказов либо null.
            return response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<List<OrderDto>>(response.Content.ReadAsStringAsync().Result) : null;
        }
        async Task<ObservableCollection<ProductDto>> GetProducts()
        {
            // Создаем клиента для посылки запроса по адресу службы.
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            // Посылаем клиенту запрос о продуктах.
            HttpResponseMessage? response = await client.GetAsync(productsControllerPath);
            // Если ответ не получен, возвращаем пустую коллекцию. 
            if (!response.IsSuccessStatusCode) return new();
            // Получаем полный список продуктов.
            List<ProductDto>? products = JsonConvert.DeserializeObject<List<ProductDto>>(response.Content.ReadAsStringAsync().Result);
            //Возвращаем полученую из базы данных коллекцию продуктов.
            return products == null ? new() : new ObservableCollection<ProductDto>(products);
        }
        async Task ResetOrders()
        {
            // Получаем из базы список заказов или null.
            List<OrderDto>? orderDtos = await GetOrders();
            // Создаем коллекцию заказов, если список существует.
            Orders = orderDtos != null ? new ObservableCollection<OrderDto>(orderDtos) : new();
        }
        async void Upload()
        {
            await ResetOrders();
            // Подгружаем продукты из базы в память.
            Products = await GetProducts();
        }
        public async Task UploadOrders()
        {
            // Загружаем список заказов из базы, если он там есть и доступен.
            List<OrderDto>? orderDtos = await GetOrders();
            // Уходим при отсутствии списка.
            if (orderDtos == null) return;
            // Формируем коллекцию заказов для размещения в GUI.
            orders = new ObservableCollection<OrderDto>(orderDtos);
            // Если выделенного заказа нет, то уходим.
            if (selectedOrder == null) return;
            // Определяем выделенный элемент в новой версии списка заказов.
            selectedOrder = orderDtos.Find(o => o.Id == selectedOrder.Id);
            // Если в новой версии списка нет прежнего выделенного заказа или список элементов заказа выделенного заказа пустой.
            if (selectedOrder == null || selectedOrder.OrderElements.Count == 0)
            {
                // Очищаем таблицу списка элементов заказов выделенного элемента.
                OrderItems.Clear();
                // Если в новой версии списка нет прежнего выделенного заказа.
                if (selectedOrder == null)
                {
                    // Обновляем GUI таблицы заказа.
                    Orders = orders;
                }
            }
            else
            {
                // Если в новой версии списка есть выделенный заказ, то обновляем список его элементов.
                OrderItems = new ObservableCollection<OrderItemDto>(selectedOrder.OrderElements);
            }
        }
        #region Order Grid Manipulations
        private void OrderSelection(object? e)
        {
            if (e == null || e is not DataGrid grid || grid.SelectedItem == null)
                return;
            // Фиксируем индекс выделенной строки таблицы заказов для дальнейшего возвращения фокуса.
            lastSelectedOrderRowIndx = grid.SelectedIndex;
            // Запоминаем ссылку на таблицу заказов.
            orderGrid = grid;
            // Фиксируем заказ, который отвечает выделенной строке.
            selectedOrder = grid.SelectedItem is OrderDto order ? order : null;
            if (selectedOrder == null)
            {
                OrderItems.Clear();
                return;
            }
            // Формируем поля нового заказа, если это новая строка.
            if (selectedOrder.Id == 0)
            {
                newRowIndex = lastSelectedOrderRowIndx;
                selectedOrder.OrderElements = new List<OrderItemDto> { new OrderItemDto() };
            }
            else
            {
                // Обновляем таблицу элементов выделенного заказа.
                OrderItems = new ObservableCollection<OrderItemDto>(selectedOrder.OrderElements);
            }
        }
        private async void RemoveSelectedOrder(object? e)
        {
            if (selectedOrder == null || lastSelectedOrderRowIndx == 0 && Orders.Count == 1)
                return;
            // Создаем клиента для посылки команд по адресу службы, обрабатывающей сообщения.
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            // Удаляем заказ из базы.
            HttpResponseMessage response = await client.DeleteAsync(ordersControllerPath + $"/{selectedOrder.Id}");
            response.EnsureSuccessStatusCode();
            // Убираем выделенный заказ из списка всех заказов.
            Orders.Remove(selectedOrder);
            // Обновляем таблицу заказов.
            await ResetOrders();
            // Переводим фокус на предыдущий или первый заказ.
            if (e != null && e is DataGrid dataGrid)
            {
                dataGrid.SelectedIndex = lastSelectedOrderRowIndx;
            }
        }
        private async void OrderRowEditEnd(object? e)
        {
            if (selectedOrder == null)
                return;
            // Создаем клиента для посылки команд по адресу службы, обрабатывающей сообщения.
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            // Посылаем запрос на создание нового заказа или обновление имеющегося.
            HttpResponseMessage? response = selectedOrder.Id == 0 ? await client.PostAsJsonAsync(ordersControllerPath, selectedOrder) :
                await client.PutAsJsonAsync(ordersControllerPath + $"/{selectedOrder.Id}", selectedOrder);
            response.EnsureSuccessStatusCode();
            // Обновляем таблицу заказов.
            await ResetOrders();
            // Переводим фокус на предыдущий или первый заказ.
            if (e != null && e is DataGrid dataGrid)
            {
                dataGrid.SelectedIndex = newRowIndex != 0 ? newRowIndex : lastSelectedOrderRowIndx;
                newRowIndex = 0;
            }
        }
        #endregion
        private void OrderItemSelection(object? e)
        {
            if (e == null || e is not DataGrid grid || grid.SelectedItem == null || grid.SelectedItem is not OrderItemDto orderItem)
                return;
            selectedOrderItem = orderItem;
            if (selectedOrderItem == null) return;
        }
        async Task UpdateOrderItem()
        {
            if (selectedOrder == null || selectedOrderItem == null) return;
            // Создаем клиента для посылки сообщений по адресу службы, обрабатывающей сообщения.
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            // Обновляем выделенный элемент заказа.
            HttpResponseMessage? response = await client.PutAsJsonAsync(orderItemsControllerPath + $"/{selectedOrderItem.Id}", selectedOrderItem);
            response.EnsureSuccessStatusCode();
        }
        private async void ProductSelection(object? e)
        {
            if (selectedOrder == null
                || Orders.Count == 0
                || OrderItems.Count == 0
                || e == null
                || e is not DataGrid grid
                || grid.SelectedItem == null
                || selectedOrder.OrderElements == null
                || grid.SelectedItem is not ProductDto addedProduct
                || selectedOrder.OrderElements.FirstOrDefault(oi => oi.Product != null && oi.Product.Id == addedProduct.Id) != null
                )
                return;
            // Добавляем новый элемент к выделенному заказу.
            selectedOrder.OrderElements.Add(new() { Product = addedProduct, Quantity = 1 });
            // Создаем клиента для посылки сообщений по адресу службы, обрабатывающей сообщения.
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            // Обновляем выделенный заказ.
            HttpResponseMessage? response = await client.PutAsJsonAsync(ordersControllerPath + $"/{selectedOrder.Id}", selectedOrder);
            response.EnsureSuccessStatusCode();
            // Подгружаем новую версию заказов.
            await UploadOrders();
        }
        private async void RemoveOrderItem(object? e)
        {
            DataGrid? agrid = e is DataGrid grid ? grid : null;
            if (agrid == null || selectedOrder == null || selectedOrderItem == null ||
                Orders.Count == 1 && lastSelectedOrderRowIndx == 0 && OrderItems.Count == 1 && agrid.SelectedIndex == 0) return;
            // Создаем клиента для посылки сообщений по адресу службы, обрабатывающей сообщения.
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            // Удаляем элемент заказа из базы.
            HttpResponseMessage? response = await client.DeleteAsync(orderItemsControllerPath + $"/{selectedOrderItem.Id}");
            response.EnsureSuccessStatusCode();
            // Обновляем GUI
            //// Загружаем список заказов из базы, если он там есть и доступен.
            await UploadOrders();
            // Если удалены все элементы текущего заказа, то переводим фокус либо на предыдущий заказ, если он не верхний.
            if (orderGrid != null && OrderItems.Count == 0)
            {
                orderGrid.SelectedIndex = Orders.Count > 0 ? lastSelectedOrderRowIndx > 0 ? lastSelectedOrderRowIndx - 1 : Orders.Count - 1 : 0;
            }
        }
        private async void OrderItemRowEditEnd(object? e)
        {
            await UpdateOrderItem();
            await UploadOrders();
        }
    }
}