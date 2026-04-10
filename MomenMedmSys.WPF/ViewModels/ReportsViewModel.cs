using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MomenMedmSys.Services;
using MomenMedmSys.WPF.ViewModels.Base;

namespace MomenMedmSys.WPF.ViewModels
{
    public partial class ReportsViewModel : ViewModelBase
    {
        private readonly IDashboardService _dashboardService;

        public ReportsViewModel(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
            Title = "Reports";
            LoadReportCommand.Execute(null);
        }

        [ObservableProperty] private string _reportSummary = string.Empty;

        [RelayCommand]
        private async Task LoadReport()
        {
            IsLoading = true;
            try
            {
                var stats = await _dashboardService.GetDashboardStatsAsync();

                ReportSummary = $"MomenMedmSys Report - Generated {DateTime.Now:yyyy-MM-dd HH:mm}\n\n" +
                    $"=== DEVICE INVENTORY ===\n" +
                    $"Total Devices: {stats.TotalDevices}\n" +
                    $"Active: {stats.ActiveDevices}\n" +
                    $"Under Maintenance: {stats.UnderMaintenanceDevices}\n" +
                    $"Out of Service: {stats.OutOfServiceDevices}\n" +
                    $"Total Asset Value: ${stats.TotalAssetValue:N2}\n\n" +
                    $"=== MAINTENANCE ===\n" +
                    $"Scheduled: {stats.ScheduledMaintenanceCount}\n" +
                    $"Overdue: {stats.OverdueMaintenanceCount}\n\n" +
                    $"=== CALIBRATION ===\n" +
                    $"Overdue: {stats.OverdueCalibrationCount}\n" +
                    $"Upcoming (30 days): {stats.UpcomingCalibrationCount}\n\n" +
                    $"=== RISK INCIDENTS ===\n" +
                    $"Open: {stats.OpenIncidentsCount}\n" +
                    $"Critical: {stats.CriticalIncidentsCount}\n\n" +
                    $"=== WORK ORDERS ===\n" +
                    $"Open: {stats.OpenWorkOrdersCount}\n" +
                    $"Overdue: {stats.OverdueWorkOrdersCount}\n\n" +
                    $"=== SPARE PARTS ===\n" +
                    $"Low Stock Alerts: {stats.LowStockPartsCount}\n" +
                    $"Total Inventory Value: ${stats.TotalInventoryValue:N2}\n";

                StatusMessage = "Report generated";
            }
            catch (Exception ex) { ReportSummary = $"Error generating report: {ex.Message}"; }
            finally { IsLoading = false; }
        }
    }
}
