using Entities;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp.Commands;

namespace WpfApp.ViewModels
{
    class IngredientsViewModel:ViewModelBase
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
        /// Хранит ссылку на объект клиента, связанного со службой API.
        /// </summary>
        private readonly RestClient restClient = new(apiAddress);
        /// <summary>
        /// Хранит ссылку на текущий выделенный объект модели.
        /// </summary>
        private Ingredient? ingredient;
        /// <summary>
        /// Хранит ссылку на коллекцию объектов модели.
        /// </summary>
        private ObservableCollection<Ingredient>? ingredients = new();
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
        public ObservableCollection<Ingredient>? Ingredients { get => ingredients; set { ingredients = value; RaisePropertyChanged(nameof(Ingredients)); } }
        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий выделенный объект модели.
        /// </summary>
        public Ingredient? Ingredient { get => ingredient; set { ingredient = value; RaisePropertyChanged(nameof(Ingredient)); } }
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
        public ICommand ItemRemoveCommand => itemRemoveCommand ??= new RelayCommand(ItemRemove);
        /// <summary>
        /// Устанавливает и возвращает ссылку на команду завершения редактирования строки в таблице UI.
        /// </summary>
        public ICommand ItemRowEditEndCommand => itemRowEditEndCommand ??= new RelayCommand(ItemRowEditEnd);
        #endregion
        public IngredientsViewModel()
        {
            GetIngredients();
        }
        public void GetIngredients()
        {
            IRestResponse response = restClient.Get(new RestRequest(controllerPath));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<Ingredient>? ingredients = JsonConvert.DeserializeObject<List<Ingredient>>(response.Content);
                if (ingredients == null)
                    return;
                Ingredients = new ObservableCollection<Ingredient>(ingredients);
                DataSource = Ingredients;
            }
        }
        private void ItemRemove(object e)
        {
            if (ingredient == null)
                return;
            restClient.Delete(new RestRequest(controllerPath + $"/{ingredient.Id}"));
            if (Ingredients != null)
                _ = Ingredients.Remove(ingredient);
        }
        private void ItemSelection(object e)
        {
            if (e == null || e is not DataGrid grid || grid.SelectedItem == null)
                return;
            Ingredient = grid.SelectedItem is Ingredient dlv ? dlv : null;
            //MessageBox.Show($"Select {(ingredient != null ? ingredient.Id : "null")}");
        }
        public async Task Create()
        {
            //IRestRequest request = new RestRequest(ControllerPath, Method.POST)
            //{
            //    RequestFormat = DataFormat.Json
            //};
            //request.AddParameter("application/json; charset=utf-8", JsonConvert.SerializeObject(ingredient), ParameterType.RequestBody);
            ////IRestResponse response = 
            //    restClient.Execute(request);
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage? response = await client.PostAsJsonAsync(controllerPath, ingredient);
            response.EnsureSuccessStatusCode();
        }
        public async Task Edit(int id)
        {
            //IRestRequest request = new RestRequest(ControllerPath + $"/{id}", Method.PUT)
            //{
            //    RequestFormat = DataFormat.Json
            //};
            //request.AddParameter("application/json; charset=utf-8", JsonConvert.SerializeObject(ingredient), ParameterType.RequestBody);
            ////IRestResponse response = 
            //restClient.Execute(request);
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage? response = await client.PutAsJsonAsync(controllerPath + $"/{id}", ingredient);
            response.EnsureSuccessStatusCode();
        }
        private async Task Commit(int id)
        {
            if (Ingredient != null && (string.IsNullOrEmpty(Ingredient.Name) || string.IsNullOrEmpty(Ingredient.Name.Trim())))
            {
                Ingredient.Name = "Noname";
            }
            if (id == 0)
            {
                await Create();
                GetIngredients();
            }
            else
                await Edit(id);
        }
        private async void ItemRowEditEnd(object e)
        {
            if (Ingredient == null || e == null || e is not DataGrid grid)
            {
                return;
            }
            await Commit(Ingredient.Id);
            grid.Items.Refresh();
        }
    }
}
