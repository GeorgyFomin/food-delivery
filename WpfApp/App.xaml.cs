using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfApp.ViewModels;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MainViewModel mainViewModel = new();
            MainWindow mainWindow = new() { DataContext = mainViewModel };
            mainWindow.Show();
            base.OnStartup(e);
        }

    }
}
