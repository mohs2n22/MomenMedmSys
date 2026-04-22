using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using MomenMedmSys.WPF.ViewModels;

namespace MomenMedmSys.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (App.ServiceProvider == null)
                {
                    MessageBox.Show("ServiceProvider not initialized", "Error");
                    return;
                }

                DataContext = App.ServiceProvider.GetRequiredService<MainViewModel>();

                if (DataContext is MainViewModel vm)
                {
                    vm.NavigateCommand.Execute(0);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"MainWindow error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavItemClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem item && DataContext is MainViewModel vm)
            {
                var index = SidebarList.Items.IndexOf(item.DataContext);
                if (index == -1) // Fallback just in case it's a Content or DataContext mismatch
                {
                    index = SidebarList.Items.IndexOf(item.Content);
                }
                
                if (index >= 0)
                {
                    vm.NavigateCommand.Execute(index);
                }
            }
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
