using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp.Commands;
using System.Linq;
using UseCases.API.Dto;

namespace WpfApp.ViewModels
{
    class ProductsViewModel : ViewModelBase
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
        /// Хранит маршрут к контроллеру Ingredients.
        /// </summary>
        private readonly string ingredientControllerPath = "api/Ingredients";
        /// <summary>
        /// Хранит ссылку на текущий выделенный объект модели.
        /// </summary>
        private ProductDto? product;
        /// <summary>
        /// Хранит ссылку на коллекцию объектов модели.
        /// </summary>
        private ObservableCollection<ProductDto>? products = new();
        /// <summary>
        /// Хранит имя текущего Product.
        /// </summary>
        private string productName = string.Empty;
        /// <summary>
        /// Хранит ссылку на текущий выделенный элемент списка IngredientsOfProduct.
        /// </summary>
        private IngredientDto? ingredient;
        /// <summary>
        /// Хранит ссылку на список ингредиентов текущего Product.
        /// </summary>
        private ObservableCollection<IngredientDto> incomingIngredients = new();
        /// <summary>
        /// Хранит ссылку на коллекцию оставшихся ингредиентов (не входящих в состав текущего продукта).
        /// </summary>
        private ObservableCollection<IngredientDto> nonIncomingIngredients = new();
        ///// <summary>
        ///// Хранит ссылку на список всех элементов таблицы Ingredients.
        ///// </summary>
        private List<IngredientDto> ingredients = new();
        /// <summary>
        /// Хранит активность таблицы ингредиентов.
        /// </summary>
        private bool ingrEnabled;
        /// <summary>
        /// Хранит ссылку на команду выделения строки в таблице UI.
        /// </summary>
        private RelayCommand? productSelectionCommand;
        /// <summary>
        /// Хранит ссылку на команду стирания записи.
        /// </summary>
        private RelayCommand? productRemoveCommand;
        /// <summary>
        /// Хранит ссылку на команду завершения редактирования записи или создания новой записи.
        /// </summary>
        private RelayCommand? productRowEditEndCommand;
        /// <summary>
        /// Хранит ссылку на команду стирания ингредиента.
        /// </summary>
        private RelayCommand? ingrRemoveCommand;
        /// <summary>
        /// Хранит ссылку на команду выделения ингредиента.
        /// </summary>
        private RelayCommand? ingrSelectionCommand;
        /// <summary>
        /// Хранит ссылку на команду завершения редактирования ингредиента.
        /// </summary>
        private RelayCommand? ingrRowEditEndCommand;
        /// <summary>
        /// Хранит ссылку на команду выделения оставшегося ингредиента (не входящих в состав текущего продукта).
        /// </summary>
        private RelayCommand? nonIncomingIngrSelectionCommand;
        #endregion
        #region Properties
        /// <summary>
        /// Устанавливает и возвращает ссылку на коллекцию продуктов.
        /// </summary>
        public ObservableCollection<ProductDto>? Products { get => products; set { products = value; RaisePropertyChanged(nameof(Products)); } }
        /// <summary>
        /// Устанавливает и возвращает ссылку на выделенный продукт.
        /// </summary>
        public ProductDto? Product
        {
            get => product;
            set
            {
                product = value;
                RaisePropertyChanged(nameof(Product));
                if (Product == null || Product.Id == 0)
                {
                    ProductName = string.Empty;
                    // Дезактивируем таблицу ингредиентов, входящих в состав продукта.
                    IngrEnabled = false;
                    // Очищаем таблицы ингредиентов.
                    IncomingIngredients.Clear();
                    NonIncomingIngredients.Clear();
                }
                else
                {
                    ProductName = Product.Name;//?? string.Empty;
                    // Формируем список ингредиентов, входящих в состав продукта.
                    IncomingIngredients = new ObservableCollection<IngredientDto>();
                    foreach (ProductIngredientDto productIngredient in Product.ProductsIngredients)
                    {
                        IngredientDto? ingredient = Ingredients.Find(i => i.Id == productIngredient.IngredientId);
                        if (ingredient != null)
                            IncomingIngredients.Add(ingredient);
                    }
                    // Формируем список оставшихся ингредиентов, не входящих в состав продукта
                    NonIncomingIngredients = new ObservableCollection<IngredientDto>();
                    foreach (IngredientDto ingredientDto in Ingredients)
                    {
                        if (ingredientDto.ProductsIngredients == null || ingredientDto.ProductsIngredients.Find(pi => pi.ProductId == Product.Id) == default)
                            NonIncomingIngredients.Add(ingredientDto);
                    }
                    // Активируем таблицу ингредиентов, входящих в состав продукта.
                    IngrEnabled = true;
                }
            }
        }
        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий выделенный объект модели.
        /// </summary>
        public IngredientDto? Ingredient { get => ingredient; set { ingredient = value; RaisePropertyChanged(nameof(Ingredient)); } }
        /// <summary>
        /// Устанавливает и возвращает ссылку на список ингредиентов.
        /// </summary>
        public List<IngredientDto> Ingredients { get => ingredients; set { ingredients = value; RaisePropertyChanged(nameof(Ingredients)); } }
        /// <summary>
        /// Устанавливает и возвращает ссылку на коллекцию ингредиентов, входящих в состав текущего продукта.
        /// </summary>
        public ObservableCollection<IngredientDto> IncomingIngredients
        {
            get => incomingIngredients;
            set { incomingIngredients = value; RaisePropertyChanged(nameof(IncomingIngredients)); }
        }
        /// <summary>
        /// Устанавливает и возвращает ссылку на коллекцию оставшихся ингредиентов (не входящих в состав текущего продукта).
        /// </summary>
        public ObservableCollection<IngredientDto> NonIncomingIngredients
        {
            get => nonIncomingIngredients; set { nonIncomingIngredients = value; RaisePropertyChanged(nameof(NonIncomingIngredients)); }
        }
        /// <summary>
        /// Устанавливает и возвращает имя текущего продукта.
        /// </summary>
        public string ProductName { get => productName; set { productName = value ?? string.Empty; RaisePropertyChanged(nameof(ProductName)); } }
        /// <summary>
        /// Устанавливает и возвращает активность таблицы ингредиентов текущего продукта.
        /// </summary>
        public bool IngrEnabled { get => ingrEnabled; set { ingrEnabled = value; RaisePropertyChanged(nameof(IngrEnabled)); } }
        /// <summary>
        /// Устанавливает и возвращает ссылку на команду выделения текущего продукта.
        /// </summary>
        public ICommand ProductSelectionCommand => productSelectionCommand ??= new RelayCommand(ProductSelection);
        /// <summary>
        /// Устанавливает и возвращает ссылку на команду стирания текущего продукта.
        /// </summary>
        public ICommand ProductRemoveCommand => productRemoveCommand ??= new RelayCommand(ProductRemoveAsync);
        /// <summary>
        /// Устанавливает и возвращает ссылку на команду завершения редактирования продукта.
        /// </summary>
        public ICommand ProductRowEditEndCommand => productRowEditEndCommand ??= new RelayCommand(ProductRowEditEndAsync);
        /// <summary>
        /// Устанавливает и возвращает ссылку на команду выделения ингредиента текущего продукта.
        /// </summary>
        public ICommand IngrSelectionCommand => ingrSelectionCommand ??= new RelayCommand(IngrSelection);
        /// <summary>
        /// Устанавливает и возвращает ссылку на команду удаления ингредиента текущего продукта.
        /// </summary>
        public ICommand IngrRemoveCommand => ingrRemoveCommand ??= new RelayCommand(IngrRemoveAsync);
        /// <summary>
        /// Устанавливает и возвращает ссылку на команду завершения редактирования ингредиента текущего продукта.
        /// </summary>
        public ICommand IngrRowEditEndCommand => ingrRowEditEndCommand ??= new RelayCommand(IngrRowEditEndAsync);
        /// <summary>
        /// Устанавливает и возвращает ссылку на команду выделения ингредиента, не входящего в текущий продукт.
        /// </summary>
        public ICommand NonIncomingIngrSelectionCommand => nonIncomingIngrSelectionCommand ??= new RelayCommand(NonIncomingIngrSelection);
        #endregion
        public ProductsViewModel()
        {
            // Подгружаем продукты из базы в память.
            UploadProducts();
            // Подгружаем ингредиенты из базы в память.
            UploadIngredients();
        }
        #region Upload&Download Methods
        /// <summary>
        /// Загружает из базы ингредиенты и для вновь созданного ингредиента выполняет обновление списка ингредиентов текущего продукта.
        /// </summary>
        /// <param name="ingredientCreated">
        /// Флаг, определяющий есть ли вновь созданный ингредиент.
        /// </param>
        public async void UploadIngredients(bool ingredientCreated = false)
        {
            // Создаем клиента для посылки сообщений по адресу службы, обрабатывающей сообщение запроса.
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            // Посылаем клиенту запрос об ингредиентах.
            HttpResponseMessage response = await client.GetAsync(ingredientControllerPath);
            if (!response.IsSuccessStatusCode) return;
            // Сохраняем результата запроса.
            var result = response.Content.ReadAsStringAsync().Result;
            // Сохраняем полученный список ингредиентов в памяти.
            Ingredients = JsonConvert.DeserializeObject<List<IngredientDto>>(result) ?? new();
            // Этот фрагмент должен работать только при создании нового ингредиента и добавлении его к существующему продукту.
            if (ingredientCreated && Product != null && IncomingIngredients != null)
            {
                // Предполагаем что вновь созданный ингредиент оказывается в конце списка Ingredients.
                IngredientDto addedIngredient = Ingredients.Last();
                // Помещаем в последний элемент списка ингредиентов продукта ссылку на вновь созданный ингредиент.
                IncomingIngredients[^1] = addedIngredient;
                // Добавляем к списку ингредиентов, входящих в состав продукта, вновь созданный ингредиент.
                Product.ProductsIngredients.Add(new ProductIngredientDto() { ProductId = Product.Id, IngredientId = addedIngredient.Id });
            }
        }
        /// <summary>
        /// Сохраняет вновь созданный ингредиент или новую редакцию существующего ингредиента в базе данных.
        /// </summary>
        /// <param name="id">Id редактируемого ингредиента, либо 0 в случае создания нового ингредиента.</param>
        /// <returns></returns>
        private async Task SaveIngredientChanges(int id)
        {
            // Создаем клиента для посылки команд по адресу службы, обрабатывающей сообщения.
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            // Посылаем клиенту команду
            // либо на создание нового элемента Ingredient, если id элемента равен 0,
            // либо на редактирование существующего Ingredient.
            HttpResponseMessage? response = id == 0 ? await client.PostAsJsonAsync(ingredientControllerPath, Ingredient) :
                await client.PutAsJsonAsync(ingredientControllerPath + $"/{id}", Ingredient);
            response.EnsureSuccessStatusCode();
        }
        /// <summary>
        /// Обновляет список продуктов в памяти, загружая их из базы данных.
        /// </summary>
        public async void UploadProducts()
        {
            // Создаем клиента для посылки запроса по адресу службы.
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            // Посылаем клиенту запрос о продуктах.
            HttpResponseMessage response = await client.GetAsync(productControllerPath);
            if (!response.IsSuccessStatusCode) return;
            // Сохраняем результата запроса.
            var result = response.Content.ReadAsStringAsync().Result;
            // Сохраняем полученный список продуктов.
            List<ProductDto>? products = JsonConvert.DeserializeObject<List<ProductDto>>(result);
            if (products != null)
                // Сохраняем полученые из базы данных ссылки на продукты в памяти.
                Products = new ObservableCollection<ProductDto>(products);
        }
        /// <summary>
        /// Сохраняет в базе новую редакцию продукта либо вновь созданный продукт.
        /// </summary>
        /// <param name="id">Значение Id редактируемого продукта, либо 0 для вновь создаваемого продукта.</param>
        /// <returns></returns>
        private async Task SaveProductChanges(int id)
        {
            // Создаем клиента для посылки команд по адресу службы, обрабатывающей сообщения.
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            // Посылаем клиенту команду
            // либо на создание нового элемента базы Product, если id элемента равен 0,
            // либо на редактирование элемента базы Product.
            HttpResponseMessage? response = id == 0 ? await client.PostAsJsonAsync(productControllerPath, Product) :
                await client.PutAsJsonAsync(productControllerPath + $"/{id}", Product);
            response.EnsureSuccessStatusCode();
        }
        #endregion
        #region Product Handlers
        /// <summary>
        /// Убирает из базы текущий продукт и обновляет список воспроизводимых продуктов.
        /// </summary>
        /// <param name="e"></param>
        private async void ProductRemoveAsync(object? e)
        {
            if (Product == null || Products == null)
                return;
            // Создаем клиента для посылки команды по адресу службы.
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            // Посылаем команду стирания текущего продукта.
            HttpResponseMessage response = await client.DeleteAsync(productControllerPath + $"/{Product.Id}");
            response.EnsureSuccessStatusCode();
            // Убираем продукт из списка.
            _ = Products.Remove(Product);
            // Обнуляем ссылку на текущий продукт.
            Product = null;
        }
        /// <summary>
        /// Обновляет ссылку на текущий продукт.
        /// </summary>
        /// <param name="e"></param>
        private void ProductSelection(object? e)
        {
            if (e == null || e is not DataGrid grid || grid.SelectedItem == null)
                return;
            // Обновляем ссылку на текущий продукт.
            Product = grid.SelectedItem is ProductDto product ? product : null;
            //MessageBox.Show($"Select {(product != null ? product.Id : "null")}");
        }
        /// <summary>
        /// При завершении редактирования строки сохраняет в базе вновь созданный продукт либо новую редакцию выделенного продукта 
        /// и обновляет список подгружаемых из базы продуктов.
        /// </summary>
        /// <param name="e"></param>
        private async void ProductRowEditEndAsync(object? e)
        {
            if (Product == null) return;
            // Сохраняем в базе вновь созданный продукт, либо редактируем существующий.
            await SaveProductChanges(Product.Id);
            // Обновляем список продуктов в памяти.
            UploadProducts();
        }
        #endregion
        #region IncomingIngredients Handlers
        /// <summary>
        /// Удаляет из списка ингредиентов текущего продукта выбранный ингредиент.
        /// </summary>
        /// <param name="e"></param>
        private async void IngrRemoveAsync(object? e)
        {
            if (Ingredient == null || Product == null)
                return;
            // Удаляем из воспроизводимого списка ингредиентов текущего продукта выбранный ингредиент.
            _ = IncomingIngredients.Remove(Ingredient);
            // Находим ссылку на выбранный ингредиент в списке ингредиентов, входящих в состав продукта.
            ProductIngredientDto? productIngredientDto = Product.ProductsIngredients.FirstOrDefault(pi => pi.IngredientId == Ingredient.Id);
            if (productIngredientDto == null) return;
            // Удаляем из списка ингредиентов, входящих в состав текущего продукта, выбранный ингредиент.
            Product.ProductsIngredients.Remove(productIngredientDto);
            // Сохраняем новую редакцию продукта в базе.
            await SaveProductChanges(Product.Id);
            // Добавляем освободившийся ингредиент в таблицу ингредиентов, не входящих в состав продукта.
            NonIncomingIngredients.Add(Ingredient);
        }
        /// <summary>
        /// Определяет выбранный ингредиент.
        /// </summary>
        /// <param name="e"></param>
        private void IngrSelection(object? e)
        {
            if (e == null || e is not DataGrid grid || grid.SelectedItem == null)
                return;
            // Сохраняем ссылку на выбранный ингредиент.
            Ingredient = grid.SelectedItem is IngredientDto ingredient ? ingredient : null;
            //MessageBox.Show($"Select {(Ingredient != null ? Ingredient.Id : "null")}");
        }
        /// <summary>
        /// Завершает редактирование записи о текущем ингредиенте, либо посылая команду на создание нового, либо на редактирование существующего.
        /// </summary>
        /// <param name="e"></param>
        private async void IngrRowEditEndAsync(object? e)
        {
            if (Product == null || Ingredient == null) return;
            int id = Ingredient.Id;
            if (id == 0)
            {
                // Добавляем ссылку на текущий продукт ко вновь создаваемому ингредиенту.
                Ingredient.ProductsIngredients = new List<ProductIngredientDto>() { new ProductIngredientDto() { ProductId = Product.Id } };
            }
            // Добавляем ингредиент в базу, либо редактируем уже существующий.
            await SaveIngredientChanges(id);
            // Обновляем список ингредиентов в памяти.
            UploadIngredients(id == 0);
        }
        #endregion
        /// <summary>
        /// Обрабатывает выделение ингредиента в таблице ингредиентов, не входящих в состав текущего продукта.
        /// Выделенный ингредиент убирает из текущей таблицы и переносит в таблицу ингредиентов, входящих в состав текущего продукта.
        /// </summary>
        /// <param name="e"></param>
        private async void NonIncomingIngrSelection(object? e)
        {
            if (e == null || e is not DataGrid grid || grid.SelectedItem == null)
                return;
            // Фиксируем ссылку на выбранный ингредиент.
            IngredientDto? ingredientDto = grid.SelectedItem is IngredientDto ingredient ? ingredient : null;
            if (ingredientDto == null || Product == null) return;
            // Добавляем его в таблицу ингредиентов текущего продукта.
            IncomingIngredients.Add(ingredientDto);
            // Добавляем в список ингредиентов, входящих в состав текущего продукта.
            Product.ProductsIngredients.Add(new ProductIngredientDto() { ProductId = Product.Id, IngredientId = ingredientDto.Id });
            // Создаем клиента для посылки сообщений по адресу службы, обрабатывающей сообщения.
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            // Посылаем клиенту сообщение о редактировании элемента базы Product.
            HttpResponseMessage? response = await client.PutAsJsonAsync(productControllerPath + $"/{Product.Id}", Product);
            response.EnsureSuccessStatusCode();
            // Удаляем выделенный ингредиент из воспроизводимого списка ингредиентов, не входящих в состав текущего продукта.
            NonIncomingIngredients.Remove(ingredientDto);
            // Загружаем обновленны список ингредиентов в память из базы данных.
            UploadIngredients();
        }
    }
}
