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
    internal class MenusViewModel : ViewModelBase
    {
        #region Fields
        /// <summary>
        /// Хранит базовый адрес службы API, используемой для разделения запросов и команд при доступе к базе данных.
        /// </summary>
        private static readonly string apiAddress = "https://localhost:7234/";//Или http://localhost:5234/
        /// <summary>
        /// Хранит маршрут к контроллеру Menus.
        /// </summary>
        private readonly string controllerPath = "api/Menus";
        /// <summary>
        /// Хранит маршрут к контроллеру MenuItems.
        /// </summary>
        private readonly string itemControllerPath = "api/MenuItems";
        /// <summary>
        /// Хранит маршрут к контроллеру Products.
        /// </summary>
        private readonly string productControllerPath = "api/Products";
        /// <summary>
        /// Хранит ссылку на текущий выделенный объект модели.
        /// </summary>
        private ProductDto? addedProduct;
        /// <summary>
        /// Хранит ссылку на коллекцию объектов модели.
        /// </summary>
        private ObservableCollection<ProductDto> nonIncomingProducts = new();
        /// <summary>
        /// Хранит ссылку на текущий выделенный объект модели.
        /// </summary>
        private MenuItemDto? removedMenuItem;
        /// <summary>
        /// Хранит ссылку на коллекцию объектов модели.
        /// </summary>
        private ObservableCollection<MenuItemDto> menuItems = new();
        /// <summary>
        /// Хранит ссылку на текущий выделенный объект модели.
        /// </summary>
        private MenuDto? selectedMenu;
        /// <summary>
        /// Хранит ссылку на коллекцию объектов модели.
        /// </summary>
        private ObservableCollection<MenuDto> menus = new();
        /// <summary>
        /// Хранит ссылку на команду выделения строки в таблице UI.
        /// </summary>
        private RelayCommand? itemSelectionCommand;
        /// <summary>
        /// Хранит ссылку на команду стирания записи.
        /// </summary>
        private RelayCommand? itemRemoveCommand;
        private RelayCommand? menuItemSelectionCommand;
        private RelayCommand? productSelectionCommand;
        #endregion
        #region Properties
        /// <summary>
        /// Устанавливает и возвращает ссылку на коллекцию продуктов.
        /// </summary>
        public ObservableCollection<ProductDto> NonIncomingProducts
        {
            get => nonIncomingProducts;
            set { nonIncomingProducts = value; RaisePropertyChanged(nameof(NonIncomingProducts)); }
        }
        /// <summary>
        /// Устанавливает и возвращает ссылку на выделенный продукт.
        /// </summary>
        public ProductDto? AddedProduct
        {
            get => addedProduct;
            set
            {
                addedProduct = value;
                RaisePropertyChanged(nameof(AddedProduct));
            }
        }
        public ObservableCollection<MenuItemDto> MenuItems { get => menuItems; set { menuItems = value; RaisePropertyChanged(nameof(MenuItems)); } }
        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий выделенный объект модели.
        /// </summary>
        public MenuItemDto? RemovedMenuItem { get => removedMenuItem; set { removedMenuItem = value; RaisePropertyChanged(nameof(RemovedMenuItem)); } }
        /// <summary>
        /// Устанавливает и возвращает коллекцию объектов модели.
        /// </summary>
        public ObservableCollection<MenuDto> Menus { get => menus; set { menus = value; RaisePropertyChanged(nameof(Menus)); } }
        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий выделенный объект модели.
        /// </summary>
        public MenuDto? SelectedMenu
        {
            get => selectedMenu; set
            {
                selectedMenu = value; RaisePropertyChanged(nameof(SelectedMenu));
                //if (Menu != null)
                if (SelectedMenu == null || SelectedMenu.MenuItems.Count == 0)
                {
                    // Очищаем таблицу.
                    MenuItems.Clear();
                    if (SelectedMenu == null)
                    {
                        Menus = menus;
                    }
                }
                else
                {
                    MenuItems = new ObservableCollection<MenuItemDto>(SelectedMenu.MenuItems);
                }
            }
        }
        /// <summary>
        /// Устанавливает и возвращает ссылку на команду выделения строки в таблице UI.
        /// </summary>
        public ICommand ItemSelectionCommand => itemSelectionCommand ??= new RelayCommand(MenuSelection);
        /// <summary>
        /// Устанавливает и возвращает ссылку на команду стирания записи в таблице UI.
        /// </summary>
        public ICommand ItemRemoveCommand => itemRemoveCommand ??= new RelayCommand(RemoveSelectedMenu);
        public ICommand MenuItemSelectionCommand => menuItemSelectionCommand ??= new RelayCommand(MenuItemSelection);
        public ICommand ProductSelectionCommand => productSelectionCommand ??= new RelayCommand(ProductSelection);
        #endregion
        public MenusViewModel()
        {
            Upload();
        }
        async void Upload()
        {
            await UploadMenus();
            // Подгружаем продукты из базы в память.
            await UploadProducts();
        }
        public async Task UploadMenus()
        {
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(controllerPath);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                List<MenuDto>? menuDtos = JsonConvert.DeserializeObject<List<MenuDto>>(result);
                if (menuDtos == null)
                    return;
                menus = new ObservableCollection<MenuDto>(menuDtos);

                if (SelectedMenu != null)
                {
                    SelectedMenu = menuDtos.Find(m => m.Id == SelectedMenu.Id);
                }
                else
                    Menus = menus;//.SkipWhile(m => m.MenuItems.Count == 0));
            }
        }
        /// <summary>
        /// Обновляет список продуктов в памяти, загружая их из базы данных.
        /// </summary>
        public async Task UploadProducts()
        {
            // Создаем клиента для посылки запроса по адресу службы.
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage? response;
            string result;
            // Запрашиваем полный список элементов меню.
            response = await client.GetAsync(itemControllerPath);

            List<MenuItemDto>? allMenuItems = null;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = response.Content.ReadAsStringAsync().Result;
                allMenuItems = JsonConvert.DeserializeObject<List<MenuItemDto>>(result);
            }
            if (allMenuItems == null)
            {
                return;
            }
            // Посылаем клиенту запрос о продуктах.
            response = await client.GetAsync(productControllerPath);
            if (!response.IsSuccessStatusCode) return;
            // Сохраняем результата запроса.
            result = response.Content.ReadAsStringAsync().Result;
            // Получаем полный список продуктов.
            List<ProductDto>? products = JsonConvert.DeserializeObject<List<ProductDto>>(result);
            if (products == null) return;
            // Выделяем продукты, которые не входят в элементы меню.
            foreach (var item in allMenuItems)
            {
                products.RemoveAll(p => item.Product != null && p.Id == item.Product.Id);
            }
            //Сохраняем полученые из базы данных ссылки на продукты в памяти.
            NonIncomingProducts = new ObservableCollection<ProductDto>(products);
        }
        private void MenuSelection(object? e)
        {
            if (e == null || e is not DataGrid grid || grid.SelectedItem == null)
                return;
            SelectedMenu = grid.SelectedItem is MenuDto menu ? menu : null;
            if (SelectedMenu == null && NonIncomingProducts.Count > 0)
            {
                CreateMenu();
                return;
            }
            Menus = menus;
        }

        private async void CreateMenu()
        {
            MenuDto menu = new();
            // Создаем клиента для посылки сообщений по адресу службы, обрабатывающей сообщения.
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage? response;
            foreach (ProductDto productDto in NonIncomingProducts)
            {
                // Добавляем к элементам меню ссылку на не использованный продуктй
                menu.MenuItems.Add(new MenuItemDto { Product = productDto });
                // Убираем этот продукт из базы Products.
                response = await client.DeleteAsync(productControllerPath + $"/{productDto.Id}");
                response.EnsureSuccessStatusCode();
            }
            // Убираем список не использованных продуктов из представления.
            NonIncomingProducts.Clear();
            // Обновляем выделеное меню.
            response = await client.PostAsJsonAsync(controllerPath, menu);
            response.EnsureSuccessStatusCode();
            Menus.Add(menu);
            Upload();
            //SelectedMenu = menu;
            //await UploadProducts();
        }

        private async void RemoveSelectedMenu(object? e)
        {
            if (SelectedMenu == null)
                return;
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            foreach (MenuItemDto menuItemDto in SelectedMenu.MenuItems)
            {
                if (menuItemDto == null || menuItemDto.Product == null)
                    continue;
                ProductDto product = menuItemDto.Product;
                // Убираем продукт из базы Products.
                HttpResponseMessage response = await client.DeleteAsync(productControllerPath + $"/{product.Id}");
                response.EnsureSuccessStatusCode();
                // Добавляем продукт в базу.
                response = await client.PostAsJsonAsync(productControllerPath, product);
                response.EnsureSuccessStatusCode();
            }
            Menus.Remove(SelectedMenu);
            // Очищаем таблицу.
            MenuItems.Clear();
            Upload();
        }
        private async Task RemoveMenuItem()
        {
            if (SelectedMenu == null || RemovedMenuItem == null)
            {
                return;
            }
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage? response;
            // Удаляем из базы Products продукты, входящие в выделенное меню.
            // Автоматически удаляются все элементы меню.
            foreach (MenuItemDto menuItemDto in SelectedMenu.MenuItems)
            {
                ProductDto? productDto = menuItemDto.Product;
                if (productDto == null)
                {
                    continue;
                }
                // Убираем продукт из базы Products.
                response = await client.DeleteAsync(productControllerPath + $"/{productDto.Id}");
                response.EnsureSuccessStatusCode();
            }
            // Добавляем продукт выделенного элемента в базу Products.
            response = await client.PostAsJsonAsync(productControllerPath, RemovedMenuItem.Product);
            response.EnsureSuccessStatusCode();
            // Удаляем из меню выделенный элемент
            SelectedMenu.MenuItems.Remove(RemovedMenuItem);
            // Обновляем выделеное меню.
            // При этом восстанавливаются все элементы меню Menu, кроме удаленного MenuItem, с продуктами, ссылки на которые они содержат.
            response = await client.PutAsJsonAsync(controllerPath + $"/{SelectedMenu.Id}", SelectedMenu);
            response.EnsureSuccessStatusCode();
        }
        private async void MenuItemSelection(object? e)
        {
            if (e == null || e is not DataGrid grid || grid.SelectedItem == null)
                return;
            RemovedMenuItem = grid.SelectedItem is MenuItemDto menuItem ? menuItem : null;
            await RemoveMenuItem();
            Upload();
        }
        async Task AddMenuItem()
        {
            if (SelectedMenu == null || AddedProduct == null)
            {
                return;
            }
            // Создаем клиента для посылки сообщений по адресу службы, обрабатывающей сообщения.
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage? response;
            // 
            foreach (MenuItemDto menuItem in SelectedMenu.MenuItems)
            {
                ProductDto? productDto = menuItem.Product;
                if (productDto == null)
                {
                    continue;
                }
                // Убираем продукт из базы Products.
                response = await client.DeleteAsync(productControllerPath + $"/{productDto.Id}");
                response.EnsureSuccessStatusCode();
            }
            // Убираем из таблицы Products базы выделенный продукт.
            response = await client.DeleteAsync(productControllerPath + $"/{AddedProduct.Id}");
            response.EnsureSuccessStatusCode();
            // Добавляем новый элемент меню в список элементов выделенного меню.
            SelectedMenu.MenuItems.Add(new() { Product = AddedProduct });
            // Обновляем выделеное меню.
            response = await client.PutAsJsonAsync(controllerPath + $"/{SelectedMenu.Id}", SelectedMenu);
            response.EnsureSuccessStatusCode();
        }
        private async void ProductSelection(object? e)
        {
            if (SelectedMenu == null || Menus.Count == 0 || MenuItems.Count == 0 || e == null || e is not DataGrid grid || grid.SelectedItem == null)
                return;
            AddedProduct = grid.SelectedItem is ProductDto ? grid.SelectedItem as ProductDto : null;
            await AddMenuItem();
            // Подгружаем новую версию элементов меню.
            Upload();
        }

    }
}
