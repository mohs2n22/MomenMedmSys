using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MomenMedmSys.WPF.ViewModels;

namespace MomenMedmSys.WPF.Controls
{
    public partial class NotificationToast : UserControl
    {
        public NotificationToast()
        {
            InitializeComponent();
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            // Handled via XAML binding to IsMouseOver trigger (OnMouseEnterCommand is private)
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            // Handled via XAML binding to IsMouseOver trigger (OnMouseLeaveCommand is private)
        }
    }
}
