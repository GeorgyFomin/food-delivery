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
        public ViewModelBase? ViewModel { get => viewModel; set { viewModel = value; RaisePropertyChanged(nameof(ViewModel)); } }
        private RelayCommand? deliveriesCommand;
        private RelayCommand? ingredientsCommand;
        private RelayCommand? productsCommand;
        public ICommand DeliveriesCommand => deliveriesCommand ??= new RelayCommand(Deliveries);
        public ICommand IngredientsCommand => ingredientsCommand ??= new RelayCommand(Ingredients);
        public ICommand ProductsCommand => productsCommand ??= new RelayCommand(Products);
        private void Deliveries(object e)
        {
            ViewModel = new DeliveriesViewModel();
        }
        private void Ingredients(object e)
        {
            ViewModel = new IngredientsViewModel();
        }
        private void Products(object commandParameter)
        {
            ViewModel = new ProductsViewModel();
        }
    }
}