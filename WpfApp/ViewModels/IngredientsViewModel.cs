using Entities;
using Entities.Domain;
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
    class IngredientsViewModel : ViewModelBase
    {
        #region Fields
        /// <summary>
        /// Хранит базовый адрес службы API, используемой для разделения запросов и команд при доступе к базе данных.
        /// </summary>
        private static readonly string apiAddress = "https://localhost:7234/";//Или http://localhost:5234/
        /// <summary>
        /// Хранит маршрут к контроллеру Ingredients.
        /// </summary>
        private readonly string ingredientControllerPath = "api/Ingredients";
        /// <summary>
        /// Хранит маршрут к контроллеру Products.
        /// </summary>
        private readonly string productControllerPath = "api/Products";
        /// <summary>
        /// Хранит ссылку на коллекцию объектов модели.
        /// </summary>
        private List<ProductDto>? products = new();
        /// <summary>
        /// Хранит ссылку на текущий выделенный объект модели.
        /// </summary>
        private IngredientDto? ingredient;
        /// <summary>
        /// Хранит ссылку на коллекцию объектов модели.
        /// </summary>
        private ObservableCollection<IngredientDto>? ingredients = new();
        /// <summary>
        /// Хранит имя текущего ингредиента.
        /// </summary>
        private string? ingredientName;
        /// <summary>
        /// Хранит ссылку на коллекцию продуктов в состав которых входит текущий выбранный ингредиент.
        /// </summary>
        private ObservableCollection<ProductDto>? productsIncludingIngredient = null;
        /// <summary>
        /// Хранит ссылку на команду выделения строки в таблице UI.
        /// </summary>
        private RelayCommand? ingredientSelectionCommand;
        /// <summary>
        /// Хранит ссылку на команду стирания записи.
        /// </summary>
        private RelayCommand? ingredientRemoveCommand;
        /// <summary>
        /// Хранит ссылку на команду завершения редактирования записи или создания новой записию
        /// </summary>
        private RelayCommand? ingredientRowEditEndCommand;
        #endregion
        #region Properties
        /// <summary>
        /// Устанавливает и возвращает имя текущего ингредиента.
        /// </summary>
        public string? IngredientName { get => ingredientName; set { ingredientName = value ?? String.Empty; RaisePropertyChanged(nameof(IngredientName)); } }
        /// <summary>
        /// Устанавливает и возвращает полный список продукиов в базе.
        /// </summary>
        public List<ProductDto>? Products { get => products; set { products = value; RaisePropertyChanged(nameof(Products)); } }
        /// <summary>
        /// Устанавливает и возвращает коллекцию ингредиентов в базе.
        /// </summary>
        public ObservableCollection<IngredientDto>? Ingredients { get => ingredients; set { ingredients = value; RaisePropertyChanged(nameof(Ingredients)); } }
        /// <summary>
        /// Устанавливает и возвращает список продуктов, в составе которых находится выбранный ингредиент.
        /// </summary>
        public ObservableCollection<ProductDto>? ProductsIncludingIngredient
        {
            get => productsIncludingIngredient; set
            {
                productsIncludingIngredient = value; RaisePropertyChanged(nameof(ProductsIncludingIngredient));
            }
        }
        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий выделенный ингредиент.
        /// </summary>
        public IngredientDto? Ingredient
        {
            get => ingredient; set
            {
                ingredient = value;
                RaisePropertyChanged(nameof(Ingredient));
                if (Ingredient == null || Ingredient.Id == 0)
                {
                    IngredientName = string.Empty;
                    if (ProductsIncludingIngredient != null)
                        ProductsIncludingIngredient.Clear();
                }
                else
                {
                    IngredientName = Ingredient.Name;
                    // Формируем список продуктов, в состав которых входит текущий ингредиент.
                    ProductsIncludingIngredient = new ObservableCollection<ProductDto>();
                    if (Ingredient.ProductsIngredients == null || Products == null) return;
                    foreach (ProductIngredientDto productIngredientDto in Ingredient.ProductsIngredients)
                    {
                        ProductDto? productDto = Products.Find(p => p.Id == productIngredientDto.ProductId);
                        if (productDto != null) ProductsIncludingIngredient.Add(productDto);
                    }
                }
            }
        }
        /// <summary>
        /// Устанавливает и возвращает ссылку на команду выделения ингредиента в таблице.
        /// </summary>
        public ICommand IngredientSelectionCommand => ingredientSelectionCommand ??= new RelayCommand(IngredientSelection);
        /// <summary>
        /// Устанавливает и возвращает ссылку на команду стирания ингредиента в таблице.
        /// </summary>
        public ICommand IngredientRemoveCommand => ingredientRemoveCommand ??= new RelayCommand(IngredientRemoveAsync);
        /// <summary>
        /// Устанавливает и возвращает ссылку на команду завершения редактирования строки в таблице ингредиентов.
        /// </summary>
        public ICommand IngredientRowEditEndCommand => ingredientRowEditEndCommand ??= new RelayCommand(IngredientRowEditEnd);
        #endregion
        public IngredientsViewModel()
        {
            // Загружаем ингредиенты в память из базы данных.
            UploadIngredients();
            // Загружаем продукты в память из базы данных.
            UploadProducts();
        }
        #region Upload&Download Methods
        /// <summary>
        /// Загружает ингредиенты в память из базы данных.
        /// </summary>
        public async void UploadIngredients()
        {
            // Создаем клиента для посылки сообщений по адресу службы, обрабатывающей сообщение запроса.
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            // Посылаем клиенту запрос об ингредиентах.
            HttpResponseMessage response = await client.GetAsync(ingredientControllerPath);
            if (response.IsSuccessStatusCode)
            {
                // Сохраняем результата запроса.
                var result = response.Content.ReadAsStringAsync().Result;
                // Сохраняем полученный список ингредиентов.
                List<IngredientDto>? ingredients = JsonConvert.DeserializeObject<List<IngredientDto>>(result);
                if (ingredients == null)
                    return;
                // Сохраняем полученный список ингредиентов в виде коллекции памяти.
                Ingredients = new ObservableCollection<IngredientDto>(ingredients);
            }
        }
        /// <summary>
        /// Сохраняет вновь созданный ингредиент или новую редакцию существующего ингредиента в базе данных.
        /// </summary>
        /// <param name="id">Id редактируемого ингредиента, либо 0 в случае создания нового ингредиента.</param>
        /// <returns></returns>
        private async Task SaveIngredientChanges(int id)
        {
            // Создаем клиента для посылки команды по адресу службы.
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            // Посылаем клиенту команду о создании нового ингредиента или редакции существующего.
            HttpResponseMessage? response = id == 0 ? await client.PostAsJsonAsync(ingredientControllerPath, Ingredient) :
                await client.PutAsJsonAsync(ingredientControllerPath + $"/{id}", Ingredient);
            response.EnsureSuccessStatusCode();
        }
        /// <summary>
        /// Загружает продукты в память из базы данных.
        /// </summary>
        public async void UploadProducts()
        {
            // Создаем клиента для посылки сообщений по адресу службы, обрабатывающей сообщение запроса.
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            // Посылаем клиенту запрос о продуктах.
            HttpResponseMessage response = await client.GetAsync(productControllerPath);
            if (response.IsSuccessStatusCode)
            {
                // Сохраняем результата запроса.
                var result = response.Content.ReadAsStringAsync().Result;
                // Сохраняем полученный список продуктов в памяти.
                Products = JsonConvert.DeserializeObject<List<ProductDto>>(result);
            }
        }
        #endregion
        #region Handlers
        /// <summary>
        /// Убирает текущий ингредиент из базы данных и из памяти.
        /// </summary>
        private async void IngredientRemoveAsync(object? e)
        {
            if (Ingredient == null)
                return;
            // Создаем клиента для посылки команды по адресу службы.
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            // Посылаем клиенту команду об уничтожении продукта.
            HttpResponseMessage response = await client.DeleteAsync(ingredientControllerPath + $"/{Ingredient.Id}");
            response.EnsureSuccessStatusCode();
            if (Ingredients != null)
            {
                // Убираем из памяти текущий ингредиент.
                _ = Ingredients.Remove(Ingredient);
            }
            if (ProductsIncludingIngredient != null)
            {
                // Очищаем список продуктов, в состав которых входит текущий ингредиент.
                ProductsIncludingIngredient.Clear();
            }
        }
        /// <summary>
        /// Выделяеет ингредиент в таблице.
        /// </summary>
        private void IngredientSelection(object? e)
        {
            if (e == null || e is not DataGrid grid || grid.SelectedItem == null)
                return;
            // Определяем ссылку на выделенный ингредиент.
            Ingredient = grid.SelectedItem is IngredientDto ingredient ? ingredient : null;
            //MessageBox.Show($"Select {(ingredient != null ? ingredient.Id : "null")}");
        }
        /// <summary>
        /// Завершает редактирование записи о текущем ингредиенте, посылая команду на создание нового, либо на редактирование существующего.
        /// </summary>
        /// <param name="e"></param>
        private async void IngredientRowEditEnd(object? e)
        {
            if (Ingredient != null)
            {
                // Создаем новый ингредиент и сохраняем в базе, либо редактируем существующий.
                await SaveIngredientChanges(Ingredient.Id);
                // Подгружаем ингредиенты из базы данных в память.
                UploadIngredients();
            }
        }
        #endregion
    }
}
