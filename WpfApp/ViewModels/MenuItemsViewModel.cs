using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using UseCases.API.Dto;
using WpfApp.Commands;

namespace WpfApp.ViewModels
{
    internal class MenuItemsViewModel : ViewModelBase
    {
        #region Fields
        /// <summary>
        /// Хранит базовый адрес службы API, используемой для разделения запросов и команд при доступе к базе данных.
        /// </summary>
        private static readonly string apiAddress = "https://localhost:7234/";//Или http://localhost:5234/
        /// <summary>
        /// Хранит маршрут к контроллеру Products.
        /// </summary>
        private readonly string productControllerPath = "api/Products";
        /// <summary>
        /// Хранит маршрут к контроллеру MenuItems.
        /// </summary>
        private readonly string controllerPath = "api/MenuItems";
        /// <summary>
        /// Хранит ссылку на текущий выделенный объект модели.
        /// </summary>
        private MenuItemDto? menuItem;
        /// <summary>
        /// Хранит ссылку на коллекцию объектов модели.
        /// </summary>
        private ObservableCollection<MenuItemDto> menuItems = new();
        /// <summary>
        /// Хранит ссылку на коллекцию объектов модели.
        /// </summary>
        private ObservableCollection<ProductDto>? nonIncomingProducts = new();
        /// <summary>
        /// Хранит ссылку на команду выделения строки в таблице UI.
        /// </summary>
        private RelayCommand? itemSelectionCommand;
        /// <summary>
        /// Хранит ссылку на команду стирания записи.
        /// </summary>
        private RelayCommand? itemRemoveCommand;
        private RelayCommand? productSelectionCommand;
        #endregion
        #region Properties
        /// <summary>
        /// Устанавливает и возвращает коллекцию объектов модели.
        /// </summary>
        public ObservableCollection<MenuItemDto> MenuItems { get => menuItems; set { menuItems = value; RaisePropertyChanged(nameof(MenuItems)); } }
        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий выделенный объект модели.
        /// </summary>
        public MenuItemDto? MenuItem
        {
            get => menuItem;
            set
            {
                menuItem = value;
                RaisePropertyChanged(nameof(MenuItem));
            }
        }
        /// <summary>
        /// Устанавливает и возвращает ссылку на коллекцию продуктов.
        /// </summary>
        public ObservableCollection<ProductDto>? NonIncomingProducts { get => nonIncomingProducts; set { nonIncomingProducts = value; RaisePropertyChanged(nameof(NonIncomingProducts)); } }
        /// <summary>
        /// Устанавливает и возвращает ссылку на команду выделения строки в таблице UI.
        /// </summary>
        public ICommand ItemSelectionCommand => itemSelectionCommand ??= new RelayCommand(ItemSelection);
        /// <summary>
        /// Устанавливает и возвращает ссылку на команду стирания записи в таблице UI.
        /// </summary>
        public ICommand ItemRemoveCommand => itemRemoveCommand ??= new RelayCommand(ItemRemoveAsync);
        public ICommand ProductSelectionCommand => productSelectionCommand ??= new RelayCommand(ProductSelection);
        #endregion
        public MenuItemsViewModel()
        {
            Upload();
        }
        private async Task UploadMenuItems()
        {
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage? response = await client.GetAsync(controllerPath);
            string result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = response.Content.ReadAsStringAsync().Result;
                List<MenuItemDto>? menuItems = JsonConvert.DeserializeObject<List<MenuItemDto>>(result);
                if (menuItems == null)
                    return;
                MenuItems = new ObservableCollection<MenuItemDto>(menuItems);
            }
        }
        public async void Upload()
        {
            await UploadMenuItems();
            // Создаем клиента для посылки запроса по адресу службы.
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            // Посылаем клиенту запрос о продуктах.
            HttpResponseMessage response = await client.GetAsync(productControllerPath);
            if (!response.IsSuccessStatusCode) return;
            // Сохраняем результата запроса.
            var result = response.Content.ReadAsStringAsync().Result;
            // Сохраняем полученный список продуктов.
            List<ProductDto>? products = JsonConvert.DeserializeObject<List<ProductDto>>(result);
            if (products == null)
            {
                return;
            }
            ProductDto? product = null;
            if (MenuItems != null)
            {
                foreach (MenuItemDto menuItemDto in MenuItems)
                {
                    if (menuItemDto.Product != null && (product = products.Find(p => p.Id == menuItemDto.Product.Id)) != null)
                    {
                        products.Remove(product);
                    }
                }
            }
            // Сохраняем полученые из базы данных ссылки на продукты в памяти.
            NonIncomingProducts = new ObservableCollection<ProductDto>(products);
        }
        private async void ItemRemoveAsync(object? e)
        {
            if (MenuItem == null || MenuItem.Product == null)
                return;
            ProductDto product = MenuItem.Product;
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            // Убираем продукт из базы Products.
            HttpResponseMessage response = await client.DeleteAsync(productControllerPath + $"/{product.Id}");
            response.EnsureSuccessStatusCode();
            // Не удается удалить элемент меню. Удаляется только продукт, который должен после этого быть восстановлен.
            //client = new() { BaseAddress = new Uri(apiAddress) };
            //response = await client.DeleteAsync(controllerPath + $"/{MenuItem.Id}");
            //response.EnsureSuccessStatusCode();
            //// Добавляем продукт в базу.
            response = await client.PostAsJsonAsync(productControllerPath, product);
            response.EnsureSuccessStatusCode();
            Upload();
        }
        private void ItemSelection(object? e)
        {
            if (e == null || e is not DataGrid grid || grid.SelectedItem == null)
                return;
            MenuItem = grid.SelectedItem is MenuItemDto dlv ? dlv : null;
            //MessageBox.Show($"Select {(delivery != null ? delivery.Id : "null")}");
        }
        private async void ProductSelection(object? e)
        {
            if (e == null || e is not DataGrid grid || grid.SelectedItem == null)
                return;
            ProductDto? product = grid.SelectedItem is ProductDto ? grid.SelectedItem as ProductDto : null;
            if (product == null)
                return;
            // Создаем клиента для посылки сообщений по адресу службы, обрабатывающей сообщения.
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            // Убираем из таблицы Products базы выделенный продукт.
            HttpResponseMessage? response = await client.DeleteAsync(productControllerPath + $"/{product.Id}");
            response.EnsureSuccessStatusCode();
            MenuItemDto menuItemDto = new() { Product = product };
            // Создаем новый элемент меню с ссылкой на этот продукт
            response = await client.PostAsJsonAsync(controllerPath, menuItemDto);
            response.EnsureSuccessStatusCode();
            MenuItems.Add(menuItemDto);
            // Подгружаем новую версию элементов меню.
            Upload();
        }

        //private async Task Commit(int id)
        //{
        //    HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
        //    HttpResponseMessage? response = id == 0 ? await client.PostAsJsonAsync(controllerPath, MenuItem) :
        //        await client.PutAsJsonAsync(controllerPath + $"/{id}", MenuItem);
        //    response.EnsureSuccessStatusCode();
        //    GetMenuItems();
        //}
        //private async void ItemRowEditEndAsync(object? e)
        //{
        //    if (MenuItem == null || MenuItem.Product == null || e == null || e is not DataGrid grid)
        //    {
        //        return;
        //    }
        //    await Commit(MenuItem.Id);
        //    grid.Items.Refresh();
        //}
    }
}
