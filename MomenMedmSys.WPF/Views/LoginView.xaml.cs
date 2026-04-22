using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MomenMedmSys.WPF.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            PasswordBox.Focus();
            Loaded += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"[LoginView] DataContext type: {DataContext?.GetType().Name ?? "null"}");
                if (DataContext is ViewModels.LoginViewModel vm)
                    System.Diagnostics.Debug.WriteLine($"[LoginView] Username: {vm.Username}, Password len: {vm.Password?.Length}, CanExecute: {vm.LoginCommand?.CanExecute(null)}");
            };
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.LoginViewModel vm)
            {
                vm.Password = ((PasswordBox)sender).Password;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void LoginView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && DataContext is ViewModels.LoginViewModel vm)
            {
                System.Diagnostics.Debug.WriteLine("[LoginView] Enter pressed");
                if (vm.LoginCommand.CanExecute(null))
                    vm.LoginCommand.Execute(null);
            }
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            // Directly read password from PasswordBox control
            var passwordFromControl = PasswordBox.Password;
            System.Diagnostics.Debug.WriteLine($"[LoginView] Direct password read, len={passwordFromControl.Length}");

            if (DataContext is ViewModels.LoginViewModel vm)
            {
                // Sync password from control to ViewModel
                vm.Password = passwordFromControl;
                System.Diagnostics.Debug.WriteLine($"[LoginView] Synced password to VM, len={vm.Password.Length}");

                if (vm.LoginCommand.CanExecute(null))
                    vm.LoginCommand.Execute(null);
                else
                    MessageBox.Show("Command cannot execute. Please try again.", "MomenMedmSys", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[LoginView] DataContext is NOT LoginViewModel!");
                MessageBox.Show("DataContext is not set correctly.", "MomenMedmSys", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
