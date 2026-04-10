using System;
using System.Windows;
using System.Windows.Controls;
using MomenMedmSys.WPF.ViewModels;
using MomenMedmSys.WPF.ViewModels.Base;

namespace MomenMedmSys.WPF.Services
{
    public interface IViewFactory
    {
        UIElement CreateViewFor(ViewModelBase viewModel);
    }

    public class ViewFactory : IViewFactory
    {
        public UIElement CreateViewFor(ViewModelBase viewModel)
        {
            var vmType = viewModel.GetType();

            if (vmType == typeof(DashboardViewModel))
                return new Views.DashboardView { DataContext = viewModel };

            if (vmType == typeof(DeviceListViewModel))
                return new Views.DeviceListView { DataContext = viewModel };

            if (vmType == typeof(DeviceFormViewModel))
                return new Views.DeviceFormView { DataContext = viewModel };

            if (vmType == typeof(MaintenanceViewModel))
                return new Views.MaintenanceView { DataContext = viewModel };

            if (vmType == typeof(MaintenanceFormViewModel))
                return new Views.MaintenanceFormView { DataContext = viewModel };

            if (vmType == typeof(CalibrationViewModel))
                return new Views.CalibrationView { DataContext = viewModel };

            if (vmType == typeof(CalibrationFormViewModel))
                return new Views.CalibrationFormView { DataContext = viewModel };

            if (vmType == typeof(SparePartsViewModel))
                return new Views.SparePartsView { DataContext = viewModel };

            if (vmType == typeof(SparePartFormViewModel))
                return new Views.SparePartFormView { DataContext = viewModel };

            if (vmType == typeof(RiskManagementViewModel))
                return new Views.RiskManagementView { DataContext = viewModel };

            if (vmType == typeof(WorkOrdersViewModel))
                return new Views.WorkOrdersView { DataContext = viewModel };

            if (vmType == typeof(StaffViewModel))
                return new Views.StaffView { DataContext = viewModel };

            if (vmType == typeof(ElectricalSafetyViewModel))
                return new Views.ElectricalSafetyView { DataContext = viewModel };

            if (vmType == typeof(NetworkDevicesViewModel))
                return new Views.NetworkDevicesView { DataContext = viewModel };

            if (vmType == typeof(ReportsViewModel))
                return new Views.ReportsView { DataContext = viewModel };

            if (vmType == typeof(AdminControlPanelViewModel))
                return new Views.AdminControlPanelView { DataContext = viewModel };

            return new TextBlock {
                Text = $"No view registered for {vmType.Name}",
                FontSize = 18,
                Foreground = System.Windows.Media.Brushes.Red,
                FontWeight = FontWeights.Bold,
                TextWrapping = TextWrapping.Wrap
            };
        }
    }
}
