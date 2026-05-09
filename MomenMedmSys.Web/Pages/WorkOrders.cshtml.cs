using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MomenMedmSys.Web.Pages
{
    public class WorkOrdersModel : PageModel
    {
        private readonly IWorkOrderService _workOrderService;
        private readonly IDeviceService _deviceService;

        public WorkOrdersModel(IWorkOrderService workOrderService, IDeviceService deviceService)
        {
            _workOrderService = workOrderService;
            _deviceService = deviceService;
        }

        public List<WorkOrder> WorkOrderList { get; set; } = new List<WorkOrder>();
        public Dictionary<int, string> DeviceNames { get; set; } = new Dictionary<int, string>();
        public string? StatusFilter { get; set; }
        public WorkOrderPriority? PriorityFilter { get; set; }

        public async Task OnGetAsync(string status = "", WorkOrderPriority? priority = null)
        {
            StatusFilter = status;
            PriorityFilter = priority;

            var allOrders = await _workOrderService.GetAllWorkOrdersAsync();
            var allDevices = await _deviceService.GetAllDevicesAsync();
            DeviceNames = allDevices.ToDictionary(d => d.Id, d => d.DeviceName);

            var filtered = allOrders.AsQueryable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<WorkOrderStatus>(status, out var workOrderStatus))
            {
                filtered = filtered.Where(w => w.Status == workOrderStatus).AsQueryable();
            }

            if (priority.HasValue)
            {
                filtered = filtered.Where(w => w.Priority == priority.Value).AsQueryable();
            }

            WorkOrderList = filtered.OrderByDescending(w => w.ReportDate).ToList();
        }
    }
}
