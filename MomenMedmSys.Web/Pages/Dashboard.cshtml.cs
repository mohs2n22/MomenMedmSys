using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MomenMedmSys.Services;
using MomenMedmSys.Web.Models;
using System.Threading.Tasks;

namespace MomenMedmSys.Web.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly IDashboardService _dashboardService;

        public DashboardModel(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public DashboardViewModel? ViewModel { get; set; }

        public async Task OnGetAsync()
        {
            var stats = await _dashboardService.GetDashboardStatsAsync();
            ViewModel = new DashboardViewModel
            {
                TotalDevices = stats.TotalDevices,
                ActiveDevices = stats.ActiveDevices,
                UnderMaintenanceDevices = stats.UnderMaintenanceDevices,
                OutOfServiceDevices = stats.OutOfServiceDevices,
                ScheduledMaintenanceCount = stats.ScheduledMaintenanceCount,
                OverdueMaintenanceCount = stats.OverdueMaintenanceCount,
                OverdueCalibrationCount = stats.OverdueCalibrationCount,
                UpcomingCalibrationCount = stats.UpcomingCalibrationCount,
                OpenIncidentsCount = stats.OpenIncidentsCount,
                CriticalIncidentsCount = stats.CriticalIncidentsCount,
                OpenWorkOrdersCount = stats.OpenWorkOrdersCount,
                OverdueWorkOrdersCount = stats.OverdueWorkOrdersCount,
                LowStockPartsCount = stats.LowStockPartsCount,
                TotalInventoryValue = stats.TotalInventoryValue,
                TotalAssetValue = stats.TotalAssetValue,
                TotalMaintenanceCost = stats.TotalMaintenanceCost,
                Alerts = stats.Alerts,
                WarrantyExpiryAlerts = stats.WarrantyExpiryAlerts
            };
        }
    }
}