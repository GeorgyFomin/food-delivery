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
        /// Хранит флаг создания объекта.
        /// </summary>
        private bool isStart = true;
        /// <summary>
        /// Хранит базовый адрес службы API, используемой для разделения запросов и команд при доступе к базе данных.
        /// </summary>
        private static readonly string apiAddress = "https://localhost:7234/";//Или http://localhost:5234/
        /// <summary>
        /// Хранит маршрут к контроллеру Menus.
        /// </summary>
        private readonly string menusControllerPath = "api/Menus";
        /// <summary>
        /// Хранит маршрут к контроллеру MenuItems.
        /// </summary>
        private readonly string itemControllerPath = "api/MenuItems";
        /// <summary>
        /// Хранит маршрут к контроллеру Products.
        /// </summary>
        private readonly string productControllerPath = "api/Products";
        /// <summary>
        /// Хранит ссылку на коллекцию объектов модели.
        /// </summary>
        private ObservableCollection<ProductDto> nonIncomingProducts = new();
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
        /// Хранит ссылку на таблицу с меню.
        /// </summary>
        private DataGrid? menuGrid;
        /// <summary>
        /// Хранит ссылку на команду выделения строки в таблице UI.
        /// </summary>
        private RelayCommand? menuSelectionCommand;
        /// <summary>
        /// Хранит ссылку на команду стирания записи.
        /// </summary>
        private RelayCommand? menuRemoveCommand;
        private RelayCommand? menuItemSelectionCommand;
        private RelayCommand? productSelectionCommand;
        private RelayCommand? menuGridLoadingRowCommand;
        #endregion
        #region Properties
        /// <summary>
        /// Устанавливает и возвращает ссылку на коллекцию продуктов.
        /// </summary>
        public ObservableCollection<ProductDto> Products
        {
            get => nonIncomingProducts;
            set { nonIncomingProducts = value; RaisePropertyChanged(nameof(Products)); }
        }
        /// <summary>
        /// Устанавливает и возвращает ссылку на коллекцию элементов меню.
        /// </summary>
        public ObservableCollection<MenuItemDto> MenuItems { get => menuItems; set { menuItems = value; RaisePropertyChanged(nameof(MenuItems)); } }
        /// <summary>
        /// Устанавливает и возвращает коллекцию объектов модели.
        /// </summary>
        public ObservableCollection<MenuDto> Menus { get => menus; set { menus = value; RaisePropertyChanged(nameof(Menus)); } }
        /// <summary>
        /// Устанавливает и возвращает ссылку на команду загрузки строки таблицы меню.
        /// </summary>
        public ICommand MenuGridLoadingRowCommand => menuGridLoadingRowCommand ??= new RelayCommand(e =>
        {
            if (isStart && e is not null && e is DataGrid dataGrid)
            {
                dataGrid.SelectedIndex = 0;
                isStart = false;
            }
        });
        /// <summary>
        /// Устанавливает и возвращает ссылку на команду выделения строки в таблице меню.
        /// </summary>
        public ICommand MenuSelectionCommand => menuSelectionCommand ??= new RelayCommand(MenuSelection);
        /// <summary>
        /// Устанавливает и возвращает ссылку на команду стирания записи в таблице меню.
        /// </summary>
        public ICommand MenuRemoveCommand => menuRemoveCommand ??= new RelayCommand(RemoveSelectedMenu);
        /// <summary>
        /// Устанавливает и возвращает ссылку на команду выделения записи в таблице элементов меню.
        /// </summary>
        public ICommand MenuItemSelectionCommand => menuItemSelectionCommand ??= new RelayCommand(MenuItemSelection);
        /// <summary>
        /// Устанавливает и возвращает ссылку на команду выделения записи в таблице продуктов.
        /// </summary>
        public ICommand ProductSelectionCommand => productSelectionCommand ??= new RelayCommand(ProductSelection);
        #endregion
        public MenusViewModel()
        {
            Upload();
        }
        async void Upload()
        {
            // Получаем из базы список меню или null.
            List<MenuDto>? menuDtos = await GetMenus();
            // Создаем коллекцию меню, если список существует.
            Menus = menuDtos != null ? new ObservableCollection<MenuDto>(menuDtos) : new();
            // Подгружаем продукты из базы в память.
            Products = await GetProducts();
        }
        async Task<List<MenuDto>?> GetMenus()
        {
            // Создаем клиента для посылки запроса по адресу службы.
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            // Посылаем клиенту запрос о меню.
            HttpResponseMessage response = await client.GetAsync(menusControllerPath);
            //Возвращаем полученый из базы данных список меню либо null.
            return response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<List<MenuDto>>(response.Content.ReadAsStringAsync().Result) : null;
        }
        async Task<ObservableCollection<ProductDto>> GetProducts()
        {
            // Создаем клиента для посылки запроса по адресу службы.
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            // Посылаем клиенту запрос о продуктах.
            HttpResponseMessage? response = await client.GetAsync(productControllerPath);
            // Если ответ не получен, возвращаем пустую коллекцию. 
            if (!response.IsSuccessStatusCode) return new();
            // Получаем полный список продуктов.
            List<ProductDto>? products = JsonConvert.DeserializeObject<List<ProductDto>>(response.Content.ReadAsStringAsync().Result);
            //Возвращаем полученую из базы данных коллекцию продуктов.
            return products == null ? new() : new ObservableCollection<ProductDto>(products);
        }
        public async Task UploadMenus()
        {
            // Загружаем список меню из базы, если он там есть и доступен.
            List<MenuDto>? menuDtos = await GetMenus();
            // Уходим при отсутствии списка.
            if (menuDtos == null) return;
            // Формируем коллекцию меню для размещения в GUI.
            menus = new ObservableCollection<MenuDto>(menuDtos);
            // Если выделенного меню нет, то уходим.
            if (selectedMenu == null) return;
            // Определяем выделенный элемент в новой версии списка меню.
            selectedMenu = menuDtos.Find(m => m.Id == selectedMenu.Id);
            // Если в новой версии списка нет прежнего выделенного меню или список элементов меню выделенного меню пустой.
            if (selectedMenu == null || selectedMenu.MenuItems.Count == 0)
            {
                // Очищаем таблицу списка элементов меню выделенного элемента.
                MenuItems.Clear();
                // Если в новой версии списка нет прежнего выделенного меню.
                if (selectedMenu == null)
                {
                    // Обновляем GUI таблицы меню.
                    Menus = menus;
                }
            }
            else
            {
                // Если в новой версии списка есть выделенное меню, то обновляем список его элементов.
                MenuItems = new ObservableCollection<MenuItemDto>(selectedMenu.MenuItems);
            }
        }
        private void MenuSelection(object? e)
        {
            if (e == null || e is not DataGrid grid || grid.SelectedItem == null)
                return;
            // Фиксируем индекс выделенной строки таблицы меню для дальнейшего возвращения фокуса.
            int indx = grid.SelectedIndex;
            // Запоминаем ссылку на таблицу меню.
            menuGrid = grid;
            // Фиксируем меню, которое отвечает выделенной строке.
            selectedMenu = grid.SelectedItem is MenuDto menu ? menu : null;
            // Создаем новое меню, если выделенная строка пустая.
            if (selectedMenu == null)
            {
                // Добавляем новое меню в строке с индексом indx таблицы меню.
                CreateMenu(indx);
                return;
            }
            // Обновляем таблицу элементов выделенного меню.
            MenuItems = new ObservableCollection<MenuItemDto>(selectedMenu.MenuItems);
        }
        private async void CreateMenu(int newRowIndex)
        {
            // Если база продуктов пуста, то не из чего создавать меню.
            if (Products.Count == 0) return;
            // Создаем новый экземпляр меню.
            MenuDto menu = new();
            // Заполняем его элементы меню всеми продуктами из базы.
            foreach (ProductDto productDto in Products)
            {
                // Добавляем к элементам меню ссылку на продукт.
                menu.MenuItems.Add(new MenuItemDto { Product = productDto });
            }
            // Посылаем запрос на редактирование меню.
            // Создаем клиента для посылки сообщений по адресу службы, обрабатывающей сообщения.
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            // Посылаем запрос на создание нового меню.
            HttpResponseMessage? response = await client.PostAsJsonAsync(menusControllerPath, menu);
            response.EnsureSuccessStatusCode();
            // Посылаем запрос на получение новой версии меню.
            response = await client.GetAsync(menusControllerPath);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                // Получаем новую версию меню.
                List<MenuDto>? menuDtos = JsonConvert.DeserializeObject<List<MenuDto>>(response.Content.ReadAsStringAsync().Result);
                if (menuDtos == null)
                    return;
                // Создаем коллекцию для воспроизведения в таблице.
                Menus = new ObservableCollection<MenuDto>(menuDtos);
            }
            if (menuGrid == null) return;
            // Возвращаем фокус на вновь созданную строку таблицы меню, если таблица доступна.
            menuGrid.SelectedIndex = newRowIndex;
        }
        private async void RemoveSelectedMenu(object? e)
        {
            if (selectedMenu == null)
                return;
            // Посылаем запрос на редактирование меню.
            // Создаем клиента для посылки сообщений по адресу службы, обрабатывающей сообщения.
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            // Посылаем запрос на удаление меню.
            HttpResponseMessage response = await client.DeleteAsync(menusControllerPath + $"/{selectedMenu.Id}");
            response.EnsureSuccessStatusCode();
            // Убираем выделенное меню из списка всех меню.
            Menus.Remove(selectedMenu);
            // Переводим фокус на первое меню.
            if (e != null && e is DataGrid dataGrid)
            {
                dataGrid.SelectedIndex = 0;
            }
        }
        private async void MenuItemSelection(object? e)
        {
            if (e == null
                || e is not DataGrid grid
                || grid.SelectedItem == null
                || grid.SelectedItem is not MenuItemDto menuItem)
                return;
            // Посылаем запрос на редактирование меню.
            // Создаем клиента для посылки сообщений по адресу службы, обрабатывающей сообщения.
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            // Посылаем запрос на удаление элемента меню.
            HttpResponseMessage? response = await client.DeleteAsync(itemControllerPath + $"/{menuItem.Id}");
            response.EnsureSuccessStatusCode();
            // Удаляем элемент меню из спика элементов выделенного меню.
            MenuItems.Remove(menuItem);
            // Обновляем GUI
            await UploadMenus();
            // Переводим фокус на первое меню, если текущее меню удалено.
            if (menuGrid != null && MenuItems.Count == 0)
            {
                menuGrid.SelectedIndex = 0;
            }
        }
        private async void ProductSelection(object? e)
        {
            if (selectedMenu == null
                || Menus.Count == 0
                || MenuItems.Count == 0
                || e == null
                || e is not DataGrid grid
                || grid.SelectedItem == null
                || grid.SelectedItem is not ProductDto addedProduct
                // Если среди элементов меню уже есть выдленный продукт, то он не используется в качестве нового элемента меню.
                || selectedMenu.MenuItems.Find(mi => mi.Product != null && mi.Product.Id == addedProduct.Id) != null)
                return;
            // Создаем новый элемент меню.
            MenuItemDto newMenuItem = new() { Product = addedProduct };
            // Добавляем новый элемент меню в список элементов выделенного меню.
            selectedMenu.MenuItems.Add(newMenuItem);
            // Посылаем запрос на редактирование меню.
            // Создаем клиента для посылки сообщений по адресу службы, обрабатывающей сообщения.
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            // Посылаем запрос на редактирование меню.
            HttpResponseMessage? response = await client.PutAsJsonAsync(menusControllerPath + $"/{selectedMenu.Id}", selectedMenu);
            response.EnsureSuccessStatusCode();
            // Обновляем GUI.
            await UploadMenus();
            // Убираем фокус с таблицы продуктов.
            grid.SelectedIndex = -1;
        }
    }
}
