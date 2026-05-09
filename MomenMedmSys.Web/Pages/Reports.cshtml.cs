using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MomenMedmSys.Services;
using System.Threading.Tasks;

namespace MomenMedmSys.Web.Pages
{
    public class ReportsModel : PageModel
    {
        private readonly IDashboardService _dashboardService;
        private readonly IDeviceService _deviceService;
        private readonly IWorkOrderService _workOrderService;
        private readonly IMaintenanceService _maintenanceService;

        public ReportsModel(
            IDashboardService dashboardService,
            IDeviceService deviceService,
            IWorkOrderService workOrderService,
            IMaintenanceService maintenanceService)
        {
            _dashboardService = dashboardService;
            _deviceService = deviceService;
            _workOrderService = workOrderService;
            _maintenanceService = maintenanceService;
        }

        public int TotalDevices { get; set; }
        public int ActiveDevices { get; set; }
        public int OpenWorkOrders { get; set; }
        public int OverdueWorkOrders { get; set; }
        public int ScheduledMaintenance { get; set; }
        public int OverdueMaintenance { get; set; }
        public decimal TotalAssetValue { get; set; }
        public decimal TotalMaintenanceCost { get; set; }
        public List<string> Alerts { get; set; } = new List<string>();

        public async Task OnGetAsync()
        {
            var stats = await _dashboardService.GetDashboardStatsAsync();

            TotalDevices = stats.TotalDevices;
            ActiveDevices = stats.ActiveDevices;
            OpenWorkOrders = stats.OpenWorkOrdersCount;
            OverdueWorkOrders = stats.OverdueWorkOrdersCount;
            ScheduledMaintenance = stats.ScheduledMaintenanceCount;
            OverdueMaintenance = stats.OverdueMaintenanceCount;
            TotalAssetValue = stats.TotalAssetValue;
            TotalMaintenanceCost = stats.TotalMaintenanceCost;
            Alerts = stats.Alerts;
        }
    }
}
