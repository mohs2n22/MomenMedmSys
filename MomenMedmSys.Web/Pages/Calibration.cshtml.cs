using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MomenMedmSys.Web.Pages
{
    public class CalibrationModel : PageModel
    {
        private readonly ICalibrationService _calibrationService;
        private readonly IDeviceService _deviceService;

        public CalibrationModel(ICalibrationService calibrationService, IDeviceService deviceService)
        {
            _calibrationService = calibrationService;
            _deviceService = deviceService;
        }

        public List<CalibrationRecord> CalibrationList { get; set; } = new List<CalibrationRecord>();
        public Dictionary<int, string> DeviceNames { get; set; } = new Dictionary<int, string>();
        public string? ResultFilter { get; set; }

        public async Task OnGetAsync(string result = "")
        {
            ResultFilter = result;

            var allCalibrations = await _calibrationService.GetAllRecordsAsync();
            var allDevices = await _deviceService.GetAllDevicesAsync();
            DeviceNames = allDevices.ToDictionary(d => d.Id, d => d.DeviceName);

            var filtered = allCalibrations.AsQueryable();

            if (!string.IsNullOrEmpty(result) && Enum.TryParse<CalibrationResult>(result, out var calibrationResult))
            {
                filtered = filtered.Where(c => c.Result == calibrationResult).AsQueryable();
            }

            CalibrationList = filtered.OrderByDescending(c => c.CalibrationDate).ToList();
        }
    }
}
