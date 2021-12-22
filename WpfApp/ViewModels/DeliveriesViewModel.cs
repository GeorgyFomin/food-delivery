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
    class DeliveriesViewModel:ViewModelBase
    {
        #region Fields
        /// <summary>
        /// Хранит базовый адрес службы API, используемой для разделения запросов и команд при доступе к базе данных.
        /// </summary>
        private static readonly string apiAddress = "https://localhost:7234/";//Или http://localhost:5234/
        /// <summary>
        /// Хранит ссылку на объект клиента, связанного со службой API.
        /// </summary>
        private readonly RestClient restClient = new(apiAddress);
        /// <summary>
        /// Хранит ссылку на текущий выделенный объект модели.
        /// </summary>
        private Delivery? delivery;
        /// <summary>
        /// Хранит ссылку на коллекцию объектов модели.
        /// </summary>
        private ObservableCollection<Delivery>? deliveries = new();
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
        public string ControllerPath { get; set; } = "api/Deliveries";
        /// <summary>
        /// Устанавливает и возвращает коллекцию объектов модели.
        /// </summary>
        public ObservableCollection<Delivery>? Deliveries { get => deliveries; set { deliveries = value; RaisePropertyChanged(nameof(Deliveries)); } }
        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий выделенный объект модели.
        /// </summary>
        public Delivery? Delivery { get => delivery; set { delivery = value; RaisePropertyChanged(nameof(Delivery)); } }
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
        public DeliveriesViewModel()
        {
            GetDeliveries();
        }
        public void GetDeliveries()
        {
            IRestResponse response = restClient.Get(new RestRequest(ControllerPath));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<Delivery>? deliveries = JsonConvert.DeserializeObject<List<Delivery>>(response.Content);
                if (deliveries == null)
                    return;
                Deliveries = new ObservableCollection<Delivery>(deliveries);
                DataSource = Deliveries;
            }
        }
        private void ItemRemove(object e)
        {
            if (delivery == null)
                return;
            restClient.Delete(new RestRequest(ControllerPath + $"/{delivery.Id}"));
            if (Deliveries != null)
                _ = Deliveries.Remove(delivery);
        }
        private void ItemSelection(object e)
        {
            if (e == null || e is not DataGrid grid || grid.SelectedItem == null)
                return;
            Delivery = grid.SelectedItem is Delivery dlv ? dlv : null;
            //MessageBox.Show($"Select {(delivery != null ? delivery.Id : "null")}");
        }
        public async Task Create()
        {
            //IRestRequest request = new RestRequest(ControllerPath, Method.POST)
            //{
            //    RequestFormat = DataFormat.Json
            //};
            //request.AddParameter("application/json; charset=utf-8", JsonConvert.SerializeObject(delivery), ParameterType.RequestBody);
            ////IRestResponse response = 
            //    restClient.Execute(request);
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage? response = await client.PostAsJsonAsync(ControllerPath, delivery);
            response.EnsureSuccessStatusCode();
        }
        public async Task Edit(int id)
        {
            //IRestRequest request = new RestRequest(ControllerPath + $"/{id}", Method.PUT)
            //{
            //    RequestFormat = DataFormat.Json
            //};
            //request.AddParameter("application/json; charset=utf-8", JsonConvert.SerializeObject(delivery), ParameterType.RequestBody);
            ////IRestResponse response = 
            //restClient.Execute(request);
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage? response = await client.PutAsJsonAsync(ControllerPath + $"/{id}", delivery);
            response.EnsureSuccessStatusCode();
        }
        private async Task Commit(int id)
        {
            if (Delivery != null && (string.IsNullOrEmpty(Delivery.ServiceName) || string.IsNullOrEmpty(Delivery.ServiceName.Trim())))
            {
                Delivery.ServiceName = "Noname";
            }
            if (id == 0)
            {
                await Create();
                GetDeliveries();
            }
            else
                await Edit(id);
        }
        private async void ItemRowEditEnd(object e)
        {
            if (Delivery == null || e == null || e is not DataGrid grid)
            {
                return;
            }
            await Commit(Delivery.Id);
            grid.Items.Refresh();
        }
    }
}
