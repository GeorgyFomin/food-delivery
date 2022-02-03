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
        private readonly string controllerPath = "api/Ingredients";
        /// <summary>
        /// Хранит ссылку на текущий выделенный объект модели.
        /// </summary>
        private IngredientDto? ingredient;
        /// <summary>
        /// Хранит ссылку на коллекцию объектов модели.
        /// </summary>
        private ObservableCollection<IngredientDto>? ingredients = new();
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
        #endregion
        #region Properties
        /// <summary>
        /// Устанавливает и возвращает коллекцию объектов модели.
        /// </summary>
        public ObservableCollection<IngredientDto>? Ingredients { get => ingredients; set { ingredients = value; RaisePropertyChanged(nameof(Ingredients)); } }
        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий выделенный объект модели.
        /// </summary>
        public IngredientDto? Ingredient { get => ingredient; set { ingredient = value; RaisePropertyChanged(nameof(Ingredient)); } }
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
        public ICommand ItemRowEditEndCommand => itemRowEditEndCommand ??= new RelayCommand(ItemRowEditEnd);
        #endregion
        public IngredientsViewModel()
        {
            GetIngredients();
        }
        public async void GetIngredients()
        {
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(controllerPath);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                List<IngredientDto>? ingredients = JsonConvert.DeserializeObject<List<IngredientDto>>(result);
                if (ingredients == null)
                    return;
                Ingredients = new ObservableCollection<IngredientDto>(ingredients);
                DataSource = Ingredients;
            }
        }
        private async void ItemRemoveAsync(object e)
        {
            if (Ingredient == null)
                return;
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.DeleteAsync(controllerPath + $"/{Ingredient.Id}");
            response.EnsureSuccessStatusCode();
            if (Ingredients != null)
                _ = Ingredients.Remove(Ingredient);
        }
        private void ItemSelection(object e)
        {
            if (e == null || e is not DataGrid grid || grid.SelectedItem == null)
                return;
            Ingredient = grid.SelectedItem is IngredientDto ingredientDto ? ingredientDto : null;
            //MessageBox.Show($"Select {(ingredient != null ? ingredient.Id : "null")}");
        }
        private async Task Commit(int id)
        {
            if (Ingredient != null && (string.IsNullOrEmpty(Ingredient.Name) || string.IsNullOrEmpty(Ingredient.Name.Trim())))
            {
                Ingredient.Name = "Noname";
            }
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage? response = id == 0 ? await client.PostAsJsonAsync(controllerPath, Ingredient) : 
                await client.PutAsJsonAsync(controllerPath + $"/{id}", Ingredient);
            response.EnsureSuccessStatusCode();
            GetIngredients();
        }
        private async void ItemRowEditEnd(object e)
        {
            if (Ingredient == null || e == null || e is not DataGrid grid)
            {
                return;
            }
            await Commit(Ingredient.Id);
            //grid.Items.Refresh();
        }
    }
}
