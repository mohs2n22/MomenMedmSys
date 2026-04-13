using System.Windows;
using System.Windows.Controls;

namespace MomenMedmSys.WPF.Views
{
    public partial class AdminControlPanelView : UserControl
    {
        public AdminControlPanelView() => InitializeComponent();

        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.AdminControlPanelViewModel vm && sender is PasswordBox pb)
            {
                vm.Password = pb.Password;
            }
        }
    }
}
