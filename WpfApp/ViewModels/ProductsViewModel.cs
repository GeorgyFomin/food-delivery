﻿using Entities;
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
using Entities.Domain;
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
        /// Хранит ссылку на текущий выделенный объект модели.
        /// </summary>
        private IngredientDto? ingredient;
        /// <summary>
        /// Хранит ссылку на объект-источник данных в таблице UI.
        /// </summary>
        private object? dataSource;
        /// <summary>
        /// Хранит ссылку на команду выделения строки в таблице UI.
        /// </summary>
        private RelayCommand? itemSelectionCommand;
        /// <summary>
        /// Хранит ссылку на команду стирания записи.
        /// </summary>
        private RelayCommand? itemRemoveCommand;
        /// <summary>
        /// Хранит ссылку на команду завершения редактирования записи или создания новой записию
        /// </summary>
        private RelayCommand? itemRowEditEndCommand;
        private RelayCommand? ingrRemoveCommand;
        private RelayCommand? ingrSelectionCommand;
        private RelayCommand? ingrRowEditEndCommand;
        private bool ingrEnabled;
        #endregion
        #region Properties
        /// <summary>
        /// Устанавливает и возвращает коллекцию объектов модели.
        /// </summary>
        public ObservableCollection<ProductDto>? Products { get => products; set { products = value; RaisePropertyChanged(nameof(Products)); } }
        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий выделенный объект модели.
        /// </summary>
        public ProductDto? Product
        {
            get => product; set
            {
                product = value;
                RaisePropertyChanged(nameof(Product));
                if (Product == null || Product.Id == 0 || Product.Ingredients == null)
                {
                    ProductName = string.Empty;
                    IngrEnabled = false;
                    Ingredients.Clear();
                }
                else
                {
                    ProductName = Product.Name;
                    IngrEnabled = true;
                }
            }
        }
        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий выделенный объект модели.
        /// </summary>
        public IngredientDto? Ingredient { get => ingredient; set { ingredient = value; RaisePropertyChanged(nameof(Ingredient)); } }
        private List<IngredientDto> allIngredients = new();
        public List<IngredientDto> AllIngredients { get => allIngredients; set { allIngredients = value; RaisePropertyChanged(nameof(AllIngredients)); } }
        private ObservableCollection<IngredientDto> ingredients = new();
        public ObservableCollection<IngredientDto> Ingredients
        {
            get => ingredients;
            set { ingredients = value; RaisePropertyChanged(nameof(Ingredients)); }
        }
        private string productName = string.Empty;
        public string ProductName { get => productName; set { productName = value ?? string.Empty; RaisePropertyChanged(nameof(ProductName)); } }
        public bool IngrEnabled { get => ingrEnabled; set { ingrEnabled = value; RaisePropertyChanged(nameof(IngrEnabled)); } }

        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий источник данных в таблице. 
        /// </summary>
        public object? DataSource { get => dataSource; set { dataSource = value; RaisePropertyChanged(nameof(DataSource)); } }
        /// <summary>
        /// Устанавливает и возвращает ссылку на команду выделения строки в таблице UI.
        /// </summary>
        public ICommand ItemSelectionCommand => itemSelectionCommand ??= new RelayCommand(ItemSelection);
        /// <summary>
        /// Устанавливает и возвращает ссылку на команду стирания записи в таблице UI.
        /// </summary>
        public ICommand ItemRemoveCommand => itemRemoveCommand ??= new RelayCommand(ItemRemoveAsync);
        /// <summary>
        /// Устанавливает и возвращает ссылку на команду завершения редактирования строки в таблице UI.
        /// </summary>
        public ICommand ItemRowEditEndCommand => itemRowEditEndCommand ??= new RelayCommand(ItemRowEditEndAsync);
        public ICommand IngrSelectionCommand => ingrSelectionCommand ??= new RelayCommand(IngrSelection);
        public ICommand IngrRemoveCommand => ingrRemoveCommand ??= new RelayCommand(IngrRemoveAsync);
        public ICommand IngrRowEditEndCommand => ingrRowEditEndCommand ??= new RelayCommand(IngrRowEditEndAsync);
        #endregion
        public ProductsViewModel()
        {
            GetProducts();
            ResetIngredients();
        }
        public async void ResetIngredients()
        {
            List<IngredientDto>? ingredients = null;
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(ingredientControllerPath);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                ingredients = JsonConvert.DeserializeObject<List<IngredientDto>>(result);
            }
            AllIngredients = ingredients ?? new();
            Ingredients = Product == null || ingredients == null ? new ObservableCollection<IngredientDto>() :
                new ObservableCollection<IngredientDto>(ingredients.Where(ingr => ingr.ProductId == Product.Id));
        }
        public async void GetProducts()
        {
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(productControllerPath);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                List<ProductDto>? products = JsonConvert.DeserializeObject<List<ProductDto>>(result);
                if (products == null)
                    return;
                Products = new ObservableCollection<ProductDto>(products);
                DataSource = Products;
            }
        }
        private async void ItemRemoveAsync(object e)
        {
            if (Product == null)
                return;
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.DeleteAsync(productControllerPath + $"/{Product.Id}");
            response.EnsureSuccessStatusCode();
            if (Products != null)
                _ = Products.Remove(Product);
            Product = null;
            Ingredients.Clear();
        }
        private void ItemSelection(object e)
        {
            if (e == null || e is not DataGrid grid || grid.SelectedItem == null)
                return;
            Product = grid.SelectedItem is ProductDto product ? product : null;
            if (Product != null)
                Ingredients = new ObservableCollection<IngredientDto>(AllIngredients.Where(ingr => ingr.ProductId == Product.Id));
            //MessageBox.Show($"Select {(product != null ? product.Id : "null")}");
        }
        private async Task Commit(int id)
        {
            if (Product != null && (string.IsNullOrEmpty(Product.Name) || string.IsNullOrEmpty(Product.Name.Trim())))
            {
                Product.Name = "Noname";
            }
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage? response = id == 0 ? await client.PostAsJsonAsync(productControllerPath, Product) :
                await client.PutAsJsonAsync(productControllerPath + $"/{id}", Product);
            response.EnsureSuccessStatusCode();
            GetProducts();
        }
        private async void ItemRowEditEndAsync(object e)
        {
            if (Product == null || e == null || e is not DataGrid grid)
            {
                return;
            }
            await Commit(Product.Id);
            grid.Items.Refresh();
        }
        private async void IngrRemoveAsync(object commandParameter)
        {
            if (Ingredient == null)
                return;
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.DeleteAsync(ingredientControllerPath + $"/{Ingredient.Id}");
            response.EnsureSuccessStatusCode();
            if (Ingredients != null)
            {
                _ = Ingredients.Remove(Ingredient);
                Ingredient = null;
                ResetIngredients();
            }
        }
        private void IngrSelection(object e)
        {
            if (e == null || e is not DataGrid grid || grid.SelectedItem == null)
                return;
            Ingredient = grid.SelectedItem is IngredientDto ingredient ? ingredient : null;
            //MessageBox.Show($"Select {(Ingredient != null ? Ingredient.Id : "null")}");
        }
        private async Task CommitIngr(int id)
        {
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage? response = id == 0 ? await client.PostAsJsonAsync(ingredientControllerPath, Ingredient) :
                await client.PutAsJsonAsync(ingredientControllerPath + $"/{id}", Ingredient);
            response.EnsureSuccessStatusCode();
            ResetIngredients();
        }
        private async void IngrRowEditEndAsync(object e)
        {
            if (Ingredient == null || e == null)// || e is not DataGrid grid
            {
                return;
            }
            if (Ingredient.Id == 0 && Product != null)
            {
                Ingredient.ProductId = Product.Id;
            }
            await CommitIngr(Ingredient.Id);
            //grid.Items.Refresh();
        }
    }
}
