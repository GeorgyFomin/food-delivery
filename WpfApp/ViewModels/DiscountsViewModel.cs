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
    internal class DiscountsViewModel : ViewModelBase
    {
        #region Fields
        /// <summary>
        /// Хранит базовый адрес службы API, используемой для разделения запросов и команд при доступе к базе данных.
        /// </summary>
        private static readonly string apiAddress = "https://localhost:7234/";//Или http://localhost:5234/
        /// <summary>
        /// Хранит маршрут к контроллеру Deliveries.
        /// </summary>
        private readonly string controllerPath = "api/Discounts";
        /// <summary>
        /// Хранит ссылку на текущий выделенный объект модели.
        /// </summary>
        private DiscountDto? discount;
        /// <summary>
        /// Хранит ссылку на коллекцию объектов модели.
        /// </summary>
        private ObservableCollection<DiscountDto>? discounts = new();
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
        public ObservableCollection<DiscountDto>? Discounts { get => discounts; set { discounts = value; RaisePropertyChanged(nameof(Discounts)); } }
        /// <summary>
        /// Устанавливает и возвращает ссылку на текущий выделенный объект модели.
        /// </summary>
        public DiscountDto? Discount { get => discount; set { discount = value; RaisePropertyChanged(nameof(Discount)); } }
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
        public DiscountsViewModel()
        {
            GetDiscounts();
        }
        public async void GetDiscounts()
        {
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(controllerPath);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                List<DiscountDto>? discounts = JsonConvert.DeserializeObject<List<DiscountDto>>(result);
                if (discounts == null)
                    return;
                Discounts = new ObservableCollection<DiscountDto>(discounts);
                DataSource = Discounts;
            }
        }
        private async void ItemRemoveAsync(object? e)
        {
            if (Discount == null)
                return;
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.DeleteAsync(controllerPath + $"/{Discount.Id}");
            response.EnsureSuccessStatusCode();
            if (Discounts != null)
                _ = Discounts.Remove(Discount);
        }
        private void ItemSelection(object? e)
        {
            if (e == null || e is not DataGrid grid || grid.SelectedItem == null)
                return;
            Discount = grid.SelectedItem is DiscountDto dlv ? dlv : null;
            //MessageBox.Show($"Select {(delivery != null ? delivery.Id : "null")}");
        }
        private async Task Commit(int id)
        {
            HttpClient? client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage? response = id == 0 ? await client.PostAsJsonAsync(controllerPath, Discount) :
                await client.PutAsJsonAsync(controllerPath + $"/{id}", Discount);
            //+$"/{id}"
            response.EnsureSuccessStatusCode();
            GetDiscounts();
        }
        private async void ItemRowEditEndAsync(object? e)
        {
            if (Discount == null || e == null || e is not DataGrid grid)
            {
                return;
            }
            await Commit(Discount.Id);
            grid.Items.Refresh();
        }
    }
}