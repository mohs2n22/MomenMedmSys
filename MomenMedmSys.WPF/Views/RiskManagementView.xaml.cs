using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MomenMedmSys.WPF.Views
{
    public partial class RiskManagementView : UserControl
    {
        public RiskManagementView() => InitializeComponent();

        private void ExportDropdown_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.ContextMenu != null)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.Placement = PlacementMode.Bottom;
                button.ContextMenu.IsOpen = true;
            }
        }
    }
}
