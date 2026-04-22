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

        private void Tab_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.AdminControlPanelViewModel vm && sender is RadioButton rb)
            {
                var index = rb.Name switch
                {
                    "tabAllStaff" => 0,
                    "tabAdmins" => 1,
                    "tabTechs" => 2,
                    "tabWriters" => 3,
                    "tabDoctors" => 4,
                    "tabNurses" => 5,
                    "tabLicenses" => 6,
                    "tabSystem" => 7,
                    "tabAbout" => 8,
                    _ => 0
                };
                vm.ActiveTabIndex = index;
            }
        }
    }
}
