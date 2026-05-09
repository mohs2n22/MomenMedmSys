using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MomenMedmSys.Web.Pages
{
    public class ElectricalSafetyModel : PageModel
    {
        private readonly IElectricalSafetyService _safetyService;
        private readonly IDeviceService _deviceService;

        public ElectricalSafetyModel(IElectricalSafetyService safetyService, IDeviceService deviceService)
        {
            _safetyService = safetyService;
            _deviceService = deviceService;
        }

        public List<ElectricalSafetyTest> SafetyTestList { get; set; } = new List<ElectricalSafetyTest>();
        public Dictionary<int, string> DeviceNames { get; set; } = new Dictionary<int, string>();
        public string? ResultFilter { get; set; }

        public async Task OnGetAsync(string result = "")
        {
            ResultFilter = result;

            var allTests = await _safetyService.GetAllTestsAsync();
            var allDevices = await _deviceService.GetAllDevicesAsync();
            DeviceNames = allDevices.ToDictionary(d => d.Id, d => d.DeviceName);

            var filtered = allTests.AsQueryable();

            if (!string.IsNullOrEmpty(result) && Enum.TryParse<SafetyTestResult>(result, out var testResult))
            {
                filtered = filtered.Where(t => t.OverallResult == testResult).AsQueryable();
            }

            SafetyTestList = filtered.OrderByDescending(t => t.TestDate).ToList();
        }
    }
}
