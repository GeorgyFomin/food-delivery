using System;
using System.IO;
using System.Windows.Input;
using WpfApp.Commands;

namespace WpfApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public static void Log(string report)
        {
            using TextWriter tw = File.AppendText("log.txt");
            tw.WriteLine($"{DateTime.Now}:{report}");
        }
        private ViewModelBase? viewModel;
        private RelayCommand? deliveriesCommand;
        private RelayCommand? ingredientsCommand;
        private RelayCommand? productsCommand;
        private RelayCommand? discountsCommand;
        private RelayCommand? menuItemsCommand;
        private RelayCommand? menusCommand;
        private RelayCommand? ordersCommand;
        public ViewModelBase? ViewModel { get => viewModel; set { viewModel = value; RaisePropertyChanged(nameof(ViewModel)); } }
        public ICommand DeliveriesCommand => deliveriesCommand ??= new RelayCommand(e => ViewModel = new DeliveriesViewModel());
        public ICommand IngredientsCommand => ingredientsCommand ??= new RelayCommand(e => ViewModel = new IngredientsViewModel());
        public ICommand ProductsCommand => productsCommand ??= new RelayCommand(e => ViewModel = new ProductsViewModel());
        public ICommand DiscountsCommand => discountsCommand ??= new RelayCommand(e => ViewModel = new DiscountsViewModel());
        public ICommand MenuItemsCommand => menuItemsCommand ??= new RelayCommand(e => ViewModel = new MenuItemsViewModel());
        public ICommand MenusCommand => menusCommand ??= new RelayCommand(e => ViewModel = new MenusViewModel());
        public ICommand OrdersCommand => ordersCommand ??= new RelayCommand(e => ViewModel = new OrdersViewModel());
    }
}