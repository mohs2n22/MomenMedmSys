using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MomenMedmSys.Web.Pages
{
    public class RisksModel : PageModel
    {
        private readonly IRiskService _riskService;
        private readonly IDeviceService _deviceService;

        public RisksModel(IRiskService riskService, IDeviceService deviceService)
        {
            _riskService = riskService;
            _deviceService = deviceService;
        }

        public List<RiskIncident> IncidentList { get; set; } = new List<RiskIncident>();
        public Dictionary<int, string> DeviceNames { get; set; } = new Dictionary<int, string>();
        public string? StatusFilter { get; set; }
        public string? SeverityFilter { get; set; }

        public async Task OnGetAsync(string status = "", string severity = "")
        {
            StatusFilter = status;
            SeverityFilter = severity;

            var allIncidents = await _riskService.GetAllIncidentsAsync();
            var allDevices = await _deviceService.GetAllDevicesAsync();
            DeviceNames = allDevices.ToDictionary(d => d.Id, d => d.DeviceName);

            var filtered = allIncidents.AsQueryable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<IncidentStatus>(status, out var incidentStatus))
            {
                filtered = filtered.Where(i => i.Status == incidentStatus).AsQueryable();
            }

            if (!string.IsNullOrEmpty(severity) && Enum.TryParse<SeverityLevel>(severity, out var severityLevel))
            {
                filtered = filtered.Where(i => i.Severity == severityLevel).AsQueryable();
            }

            IncidentList = filtered.OrderByDescending(i => i.IncidentDate).ToList();
        }
    }
}
