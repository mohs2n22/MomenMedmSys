using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MomenMedmSys.Web.Pages
{
    public class MedicalDevicesModel : PageModel
    {
        private readonly IDeviceService _deviceService;
        private readonly IDepartmentService _departmentService;

        public MedicalDevicesModel(IDeviceService deviceService, IDepartmentService departmentService)
        {
            _deviceService = deviceService;
            _departmentService = departmentService;
        }

        public List<MedicalDevice> DeviceList { get; set; } = new List<MedicalDevice>();
        public List<Department> Departments { get; set; } = new List<Department>();
        public string? SearchTerm { get; set; }
        public string? StatusFilter { get; set; }
        public int? DepartmentFilter { get; set; }
        public string? RiskFilter { get; set; }

        public async Task OnGetAsync(string search = "", string status = "", int? department = null, string risk = "")
        {
            SearchTerm = search;
            StatusFilter = status;
            DepartmentFilter = department;
            RiskFilter = risk;

            var allDevices = await _deviceService.GetAllDevicesAsync();
            Departments = (await _departmentService.GetAllAsync()).ToList();

            var filtered = allDevices.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                filtered = filtered.Where(d =>
                    d.DeviceName.Contains(search, System.StringComparison.OrdinalIgnoreCase) ||
                    d.DeviceCode.Contains(search, System.StringComparison.OrdinalIgnoreCase) ||
                    d.Manufacturer.Contains(search, System.StringComparison.OrdinalIgnoreCase) ||
                    d.SerialNumber.Contains(search, System.StringComparison.OrdinalIgnoreCase)
                ).AsQueryable();
            }

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<DeviceStatus>(status, out var deviceStatus))
            {
                filtered = filtered.Where(d => d.Status == deviceStatus).AsQueryable();
            }

            if (department.HasValue)
            {
                filtered = filtered.Where(d => d.DepartmentId == department.Value).AsQueryable();
            }

            if (!string.IsNullOrEmpty(risk) && Enum.TryParse<RiskClass>(risk, out var riskClass))
            {
                filtered = filtered.Where(d => d.RiskClassification == riskClass).AsQueryable();
            }

            DeviceList = filtered.ToList();
        }
    }
}
