using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using MomenMedmSys.Services;
using MomenMedmSys.WPF.ViewModels.Base;

namespace MomenMedmSys.WPF.ViewModels
{
    public partial class DashboardViewModel : ViewModelBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardViewModel(IDashboardService dashboardService)
        {
            Title = "Dashboard";
            _dashboardService = dashboardService;
            LoadStatsCommand.Execute(null);
        }

        // Device Stats
        [ObservableProperty] private int _totalDevices;
        [ObservableProperty] private int _activeDevices;
        [ObservableProperty] private int _underMaintenanceDevices;
        [ObservableProperty] private int _outOfServiceDevices;

        // Maintenance Stats
        [ObservableProperty] private int _scheduledMaintenanceCount;
        [ObservableProperty] private int _overdueMaintenanceCount;

        // Calibration Stats
        [ObservableProperty] private int _overdueCalibrationCount;
        [ObservableProperty] private int _upcomingCalibrationCount;

        // Risk Stats
        [ObservableProperty] private int _openIncidentsCount;
        [ObservableProperty] private int _criticalIncidentsCount;

        // Work Order Stats
        [ObservableProperty] private int _openWorkOrdersCount;
        [ObservableProperty] private int _overdueWorkOrdersCount;

        // Spare Parts Stats
        [ObservableProperty] private int _lowStockPartsCount;
        [ObservableProperty] private decimal _totalInventoryValue;

        // Financial
        [ObservableProperty] private decimal _totalAssetValue;
        [ObservableProperty] private decimal _totalMaintenanceCost;

        // Alerts
        [ObservableProperty] private ObservableCollection<string> _alerts = new();
        [ObservableProperty] private ObservableCollection<string> _warrantyAlerts = new();

        [RelayCommand]
        private async Task LoadStats()
        {
            IsLoading = true;
            try
            {
                var stats = await _dashboardService.GetDashboardStatsAsync();

                TotalDevices = stats.TotalDevices;
                ActiveDevices = stats.ActiveDevices;
                UnderMaintenanceDevices = stats.UnderMaintenanceDevices;
                OutOfServiceDevices = stats.OutOfServiceDevices;
                ScheduledMaintenanceCount = stats.ScheduledMaintenanceCount;
                OverdueMaintenanceCount = stats.OverdueMaintenanceCount;
                OverdueCalibrationCount = stats.OverdueCalibrationCount;
                UpcomingCalibrationCount = stats.UpcomingCalibrationCount;
                OpenIncidentsCount = stats.OpenIncidentsCount;
                CriticalIncidentsCount = stats.CriticalIncidentsCount;
                OpenWorkOrdersCount = stats.OpenWorkOrdersCount;
                OverdueWorkOrdersCount = stats.OverdueWorkOrdersCount;
                LowStockPartsCount = stats.LowStockPartsCount;
                TotalInventoryValue = stats.TotalInventoryValue;
                TotalAssetValue = stats.TotalAssetValue;
                TotalMaintenanceCost = stats.TotalMaintenanceCost;

                Alerts.Clear();
                foreach (var alert in stats.Alerts)
                    Alerts.Add(alert);

                WarrantyAlerts.Clear();
                foreach (var alert in stats.WarrantyExpiryAlerts)
                    WarrantyAlerts.Add(alert);

                StatusMessage = "Dashboard refreshed successfully";
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"Error loading stats: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
