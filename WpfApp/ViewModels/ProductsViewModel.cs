using Entities;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp.Commands;

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
        private readonly string controllerPath = "api/Products";
        /// <summary>
        /// Хранит ссылку на объект клиента, связанного со службой API.
        /// </summary>
        private readonly RestClient restClient = new(apiAddress);
        /// <summary>
        /// Хранит ссылку на текущий выделенный объект модели.
        /// </summary>
        private Product? product;
        /// <summary>
        /// Хранит ссылку на коллекцию объектов модели.
        /// </summary>
        private ObservableCollection<Product>? products = new();
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
        public ObservableCollection<Product>? Products { get => products; set { products = value; RaisePropertyChanged(nameof(Products)); } }
        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий выделенный объект модели.
        /// </summary>
        public Product? Product { get => product; set { product = value; RaisePropertyChanged(nameof(Product)); } }
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
        public ProductsViewModel()
        {
            GetProducts();
        }
        public void GetProducts()
        {
            IRestResponse response = restClient.Get(new RestRequest(controllerPath));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<Product>? products = JsonConvert.DeserializeObject<List<Product>>(response.Content);
                if (products == null)
                    return;
                Products = new ObservableCollection<Product>(products);
                DataSource = Products;
            }
        }
        private void ItemRemove(object e)
        {
            if (product == null)
                return;
            restClient.Delete(new RestRequest(controllerPath + $"/{product.Id}"));
            if (Products != null)
                _ = Products.Remove(product);
        }
        private void ItemSelection(object e)
        {
            if (e == null || e is not DataGrid grid || grid.SelectedItem == null)
                return;
            Product = grid.SelectedItem is Product product ? product : null;
            //MessageBox.Show($"Select {(product != null ? product.Id : "null")}");
        }
        public async Task Create()
        {
            //IRestRequest request = new RestRequest(controllerPath, Method.POST)
            //{
            //    RequestFormat = DataFormat.Json
            //};
            //request.AddParameter("application/json; charset=utf-8", JsonConvert.SerializeObject(product), ParameterType.RequestBody);
            ////IRestResponse response = 
            //restClient.Execute(request);
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage? response = await client.PostAsJsonAsync(controllerPath, product);
            response.EnsureSuccessStatusCode();
        }
        public async Task Edit(int id)
        {
            //IRestRequest request = new RestRequest(controllerPath + $"/{id}", Method.PUT)
            //{
            //    RequestFormat = DataFormat.Json
            //};
            //request.AddParameter("application/json; charset=utf-8", JsonConvert.SerializeObject(product), ParameterType.RequestBody);
            //IRestResponse response = restClient.Execute(request);
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage? response = await client.PutAsJsonAsync(controllerPath + $"/{id}", product);
            response.EnsureSuccessStatusCode();
        }
        private async Task Commit(int id)
        {
            if (Product != null && (string.IsNullOrEmpty(Product.Name) || string.IsNullOrEmpty(Product.Name.Trim())))
            {
                Product.Name = "Noname";
            }
            if (id == 0)
            {
                await Create();
                GetProducts();
            }
            else
                await Edit(id);
        }
        private async void ItemRowEditEnd(object e)
        {
            if (Product == null || e == null || e is not DataGrid grid)
            {
                return;
            }
            await Commit(Product.Id);
            grid.Items.Refresh();
        }
    }
}
