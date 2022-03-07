using Entities;
using Entities.Domain;
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
using UseCases.API.Dto;
using WpfApp.Commands;

namespace WpfApp.ViewModels
{
    class DeliveriesViewModel : ViewModelBase
    {
        #region Fields
        /// <summary>
        /// Хранит базовый адрес службы API, используемой для разделения запросов и команд при доступе к базе данных.
        /// </summary>
        private static readonly string apiAddress = "https://localhost:7234/";//Или http://localhost:5234/
        /// <summary>
        /// Хранит маршрут к контроллеру Deliveries.
        /// </summary>
        private readonly string controllerPath = "api/Deliveries";
        /// <summary>
        /// Хранит ссылку на текущий выделенный объект модели.
        /// </summary>
        private DeliveryDto? delivery;
        /// <summary>
        /// Хранит ссылку на коллекцию объектов модели.
        /// </summary>
        private ObservableCollection<DeliveryDto>? deliveries = new();
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
        public ObservableCollection<DeliveryDto>? Deliveries { get => deliveries; set { deliveries = value; RaisePropertyChanged(nameof(Deliveries)); } }
        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий выделенный объект модели.
        /// </summary>
        public DeliveryDto? Delivery { get => delivery; set { delivery = value; RaisePropertyChanged(nameof(Delivery)); } }
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
        #endregion
        public DeliveriesViewModel()
        {
            GetDeliveries();
        }
        public async void GetDeliveries()
        {
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(controllerPath);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                List<DeliveryDto>? deliveries = JsonConvert.DeserializeObject<List<DeliveryDto>>(result);
                if (deliveries == null)
                    return;
                Deliveries = new ObservableCollection<DeliveryDto>(deliveries);
                DataSource = Deliveries;
            }
        }
        private async void ItemRemoveAsync(object? e)
        {
            if (Delivery == null)
                return;
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.DeleteAsync(controllerPath + $"/{Delivery.Id}");
            response.EnsureSuccessStatusCode();
            if (Deliveries != null)
                _ = Deliveries.Remove(Delivery);
        }
        private void ItemSelection(object? e)
        {
            if (e == null || e is not DataGrid grid || grid.SelectedItem == null)
                return;
            Delivery = grid.SelectedItem is DeliveryDto dlv ? dlv : null;
            //MessageBox.Show($"Select {(delivery != null ? delivery.Id : "null")}");
        }
        private async Task Commit(int id)
        {
            if (Delivery != null && (string.IsNullOrEmpty(Delivery.ServiceName) || string.IsNullOrEmpty(Delivery.ServiceName.Trim())))
            {
                Delivery.ServiceName = "Noname";
            }
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage? response = id == 0 ? await client.PostAsJsonAsync(controllerPath, Delivery) :
                await client.PutAsJsonAsync(controllerPath + $"/{id}", Delivery);
            //+$"/{id}"
            response.EnsureSuccessStatusCode();
            GetDeliveries();
        }
        private async void ItemRowEditEndAsync(object? e)
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
