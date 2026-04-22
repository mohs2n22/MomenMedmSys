using System.Windows;
using System.Windows.Controls;

namespace MomenMedmSys.WPF.Views
{
    public partial class UserFormView : UserControl
    {
        public UserFormView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.UserFormViewModel vm)
            {
                vm.Password = ((PasswordBox)sender).Password;
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.UserFormViewModel vm)
            {
                vm.ConfirmPassword = ((PasswordBox)sender).Password;
            }
        }
    }
}
