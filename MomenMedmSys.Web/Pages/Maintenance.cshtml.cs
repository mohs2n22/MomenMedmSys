using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MomenMedmSys.Web.Pages
{
    public class MaintenanceModel : PageModel
    {
        private readonly IMaintenanceService _maintenanceService;
        private readonly IDeviceService _deviceService;

        public MaintenanceModel(IMaintenanceService maintenanceService, IDeviceService deviceService)
        {
            _maintenanceService = maintenanceService;
            _deviceService = deviceService;
        }

        public List<MaintenanceRecord> MaintenanceList { get; set; } = new List<MaintenanceRecord>();
        public Dictionary<int, string> DeviceNames { get; set; } = new Dictionary<int, string>();
        public string? StatusFilter { get; set; }

        public async Task OnGetAsync(string status = "")
        {
            StatusFilter = status;

            var allMaintenance = await _maintenanceService.GetAllRecordsAsync();
            var allDevices = await _deviceService.GetAllDevicesAsync();
            DeviceNames = allDevices.ToDictionary(d => d.Id, d => d.DeviceName);

            var filtered = allMaintenance.AsQueryable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<MaintenanceStatus>(status, out var maintenanceStatus))
            {
                filtered = filtered.Where(m => m.Status == maintenanceStatus).AsQueryable();
            }

            MaintenanceList = filtered.OrderByDescending(m => m.ScheduledDate).ToList();
        }
    }
}
