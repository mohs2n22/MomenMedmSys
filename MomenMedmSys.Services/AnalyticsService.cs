using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Data;

namespace MomenMedmSys.Services
{
    /// <summary>
    /// Service for advanced KPI calculations and analytics — MTBF, MTTR, equipment availability, maintenance
    /// completion rate, calibration compliance, cost analysis, trend reporting, and top-failing equipment queries.
    /// </summary>
    public interface IAnalyticsService
    {
        Task<double> GetEquipmentAvailabilityAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<double> GetMTBFAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<double> GetMTTRAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<double> GetMaintenanceCompletionRateAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<double> GetCalibrationComplianceAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<double> GetCostPerDeviceAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<Dictionary<string, int>> GetWorkOrderStatusDistributionAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<MaintenanceTrendItem>> GetMaintenanceByMonthAsync(int months = 12);
        Task<Dictionary<string, int>> GetDeviceStatusDistributionAsync();
        Task<List<DepartmentCostItem>> GetDepartmentCostComparisonAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<MonthlyIncidentItem>> GetRiskIncidentTrendAsync(int months = 12);
        Task<List<FailingEquipmentItem>> GetTopFailingEquipmentAsync(int count = 10, DateTime? startDate = null, DateTime? endDate = null);
        Task<List<WarrantyExpiryItem>> GetWarrantyExpiryTimelineAsync();
    }

    public class MaintenanceTrendItem
    {
        public string Month { get; set; } = string.Empty;
        public int PreventiveCount { get; set; }
        public int CorrectiveCount { get; set; }
        public int EmergencyCount { get; set; }
        public int Total => PreventiveCount + CorrectiveCount + EmergencyCount;
        public decimal TotalCost { get; set; }
    }

    public class DepartmentCostItem
    {
        public string DepartmentName { get; set; } = string.Empty;
        public decimal MaintenanceCost { get; set; }
        public decimal PartsCost { get; set; }
        public decimal TotalCost => MaintenanceCost + PartsCost;
        public int DeviceCount { get; set; }
    }

    public class MonthlyIncidentItem
    {
        public string Month { get; set; } = string.Empty;
        public int IncidentCount { get; set; }
    }

    public class FailingEquipmentItem
    {
        public int DeviceId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string DeviceCode { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public int MaintenanceCount { get; set; }
        public decimal TotalCost { get; set; }
        public double DowntimeHours { get; set; }
    }

    public class WarrantyExpiryItem
    {
        public string Quarter { get; set; } = string.Empty;
        public int DeviceCount { get; set; }
        public List<WarrantyDeviceItem> Devices { get; set; } = new();
    }

    public class WarrantyDeviceItem
    {
        public string DeviceName { get; set; } = string.Empty;
        public string DeviceCode { get; set; } = string.Empty;
        public DateTime WarrantyExpiryDate { get; set; }
        public string Department { get; set; } = string.Empty;
    }

    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<double> GetEquipmentAvailabilityAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var devices = await _unitOfWork.MedicalDevices.GetAllAsync();
            var activeDevices = devices.Where(d => d.IsActive);

            if (!activeDevices.Any()) return 0;

            var availableCount = activeDevices.Count(d =>
                d.Status == DeviceStatus.Active || d.Status == DeviceStatus.UnderMaintenance);

            // Calculate availability based on active vs under maintenance
            var total = activeDevices.Count();
            var underMaintenance = activeDevices.Count(d => d.Status == DeviceStatus.UnderMaintenance);
            var outOfService = activeDevices.Count(d => d.Status == DeviceStatus.OutOfService);

            // Availability = (Total - OutOfService) / Total * 100
            return total > 0 ? Math.Round((double)(total - outOfService) / total * 100, 1) : 0;
        }

        public async Task<double> GetMTBFAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var maintenanceRecords = await _unitOfWork.MaintenanceRecords.GetAllAsync();
            var devices = await _unitOfWork.MedicalDevices.GetAllAsync();

            var completedRecords = maintenanceRecords
                .Where(m => m.Status == MaintenanceStatus.Completed && m.CompletedDate.HasValue)
                .ToList();

            if (startDate.HasValue)
                completedRecords = completedRecords.Where(m => m.CompletedDate >= startDate.Value).ToList();
            if (endDate.HasValue)
                completedRecords = completedRecords.Where(m => m.CompletedDate <= endDate.Value).ToList();

            if (!completedRecords.Any()) return 0;

            // MTBF = Total operational time / Number of failures
            // Approximate: average days between failures per device
            var devicesWithFailures = completedRecords
                .GroupBy(m => m.DeviceId)
                .Where(g => g.Count() > 1)
                .ToList();

            if (!devicesWithFailures.Any())
            {
                // If no device has multiple failures, return average operational days
                var activeDevices = devices.Where(d => d.IsActive && d.Status == DeviceStatus.Active).ToList();
                if (!activeDevices.Any()) return 0;

                var totalOperationalDays = activeDevices.Sum(d =>
                    (d.InstallationDate.HasValue ? (DateTime.Now - d.InstallationDate.Value).TotalDays : 365));

                var failureCount = completedRecords.Count;
                return failureCount > 0 ? Math.Round(totalOperationalDays / failureCount, 1) : 0;
            }

            var totalDaysBetweenFailures = 0.0;
            var failureIntervals = 0;

            foreach (var group in devicesWithFailures)
            {
                var sorted = group.OrderBy(m => m.CompletedDate).ToList();
                for (int i = 1; i < sorted.Count; i++)
                {
                    var days = (sorted[i].CompletedDate!.Value - sorted[i - 1].CompletedDate!.Value).TotalDays;
                    totalDaysBetweenFailures += days;
                    failureIntervals++;
                }
            }

            return failureIntervals > 0 ? Math.Round(totalDaysBetweenFailures / failureIntervals, 1) : 0;
        }

        public async Task<double> GetMTTRAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var maintenanceRecords = await _unitOfWork.MaintenanceRecords.GetAllAsync();

            var completedRecords = maintenanceRecords
                .Where(m => m.Status == MaintenanceStatus.Completed && m.DowntimeHours.HasValue)
                .ToList();

            if (startDate.HasValue)
                completedRecords = completedRecords.Where(m => m.ScheduledDate >= startDate.Value).ToList();
            if (endDate.HasValue)
                completedRecords = completedRecords.Where(m => m.ScheduledDate <= endDate.Value).ToList();

            if (!completedRecords.Any()) return 0;

            var totalDowntime = completedRecords.Sum(m => (double)m.DowntimeHours!.Value);
            return Math.Round(totalDowntime / completedRecords.Count, 1);
        }

        public async Task<double> GetMaintenanceCompletionRateAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var maintenanceRecords = await _unitOfWork.MaintenanceRecords.GetAllAsync();

            var records = maintenanceRecords.ToList();
            if (startDate.HasValue)
                records = records.Where(m => m.ScheduledDate >= startDate.Value).ToList();
            if (endDate.HasValue)
                records = records.Where(m => m.ScheduledDate <= endDate.Value).ToList();

            if (!records.Any()) return 0;

            var completedCount = records.Count(m => m.Status == MaintenanceStatus.Completed);
            return Math.Round((double)completedCount / records.Count * 100, 1);
        }

        public async Task<double> GetCalibrationComplianceAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var calibrationRecords = await _unitOfWork.CalibrationRecords.GetAllAsync();

            var records = calibrationRecords.ToList();
            if (startDate.HasValue)
                records = records.Where(c => c.CalibrationDate >= startDate.Value).ToList();
            if (endDate.HasValue)
                records = records.Where(c => c.CalibrationDate <= endDate.Value).ToList();

            if (!records.Any()) return 0;

            var onTimeCount = records.Count(c =>
                c.Result == CalibrationResult.Pass || c.Result == CalibrationResult.PassWithAdjustment);

            return Math.Round((double)onTimeCount / records.Count * 100, 1);
        }

        public async Task<double> GetCostPerDeviceAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var maintenanceRecords = await _unitOfWork.MaintenanceRecords.GetAllAsync();
            var devices = await _unitOfWork.MedicalDevices.GetAllAsync();

            var records = maintenanceRecords.ToList();
            if (startDate.HasValue)
                records = records.Where(m => m.ScheduledDate >= startDate.Value).ToList();
            if (endDate.HasValue)
                records = records.Where(m => m.ScheduledDate <= endDate.Value).ToList();

            var totalCost = records.Sum(m => m.TotalCost);
            var activeDevices = devices.Count(d => d.IsActive);

            return activeDevices > 0 ? Math.Round((double)totalCost / activeDevices, 2) : 0;
        }

        public async Task<Dictionary<string, int>> GetWorkOrderStatusDistributionAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var workOrders = await _unitOfWork.WorkOrders.GetAllAsync();

            var filtered = workOrders.ToList();
            if (startDate.HasValue)
                filtered = filtered.Where(w => w.ReportDate >= startDate.Value).ToList();
            if (endDate.HasValue)
                filtered = filtered.Where(w => w.ReportDate <= endDate.Value).ToList();

            return filtered
                .GroupBy(w => w.Status.ToString())
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<List<MaintenanceTrendItem>> GetMaintenanceByMonthAsync(int months = 12)
        {
            var maintenanceRecords = await _unitOfWork.MaintenanceRecords.GetAllAsync();
            var cutoffDate = DateTime.Now.AddMonths(-months);

            var records = maintenanceRecords
                .Where(m => m.ScheduledDate >= cutoffDate)
                .ToList();

            var trendData = new List<MaintenanceTrendItem>();

            for (int i = months - 1; i >= 0; i--)
            {
                var month = DateTime.Now.AddMonths(-i);
                var monthStart = new DateTime(month.Year, month.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                var monthRecords = records.Where(m =>
                    m.ScheduledDate >= monthStart && m.ScheduledDate <= monthEnd).ToList();

                trendData.Add(new MaintenanceTrendItem
                {
                    Month = monthStart.ToString("MMM yyyy"),
                    PreventiveCount = monthRecords.Count(m => m.Type == MaintenanceType.Preventive),
                    CorrectiveCount = monthRecords.Count(m => m.Type == MaintenanceType.Corrective),
                    EmergencyCount = monthRecords.Count(m => m.Type == MaintenanceType.Emergency),
                    TotalCost = monthRecords.Sum(m => m.TotalCost)
                });
            }

            return trendData;
        }

        public async Task<Dictionary<string, int>> GetDeviceStatusDistributionAsync()
        {
            var devices = await _unitOfWork.MedicalDevices.GetAllAsync();
            var activeDevices = devices.Where(d => d.IsActive).ToList();

            return activeDevices
                .GroupBy(d => d.Status.ToString())
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<List<DepartmentCostItem>> GetDepartmentCostComparisonAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var devices = await _unitOfWork.MedicalDevices.GetAllAsync();
            var maintenanceRecords = await _unitOfWork.MaintenanceRecords.GetAllAsync();
            var departments = await _unitOfWork.Departments.GetAllAsync();

            var records = maintenanceRecords.ToList();
            if (startDate.HasValue)
                records = records.Where(m => m.ScheduledDate >= startDate.Value).ToList();
            if (endDate.HasValue)
                records = records.Where(m => m.ScheduledDate <= endDate.Value).ToList();

            var activeDevices = devices.Where(d => d.IsActive).ToList();

            var departmentCosts = activeDevices
                .Where(d => d.DepartmentId.HasValue)
                .GroupBy(d => d.DepartmentId!.Value)
                .Select(g =>
                {
                    var deptDevices = g.ToList();
                    var deptDeviceIds = deptDevices.Select(d => d.Id).ToHashSet();
                    var deptRecords = records.Where(m => deptDeviceIds.Contains(m.DeviceId)).ToList();

                    var department = departments.FirstOrDefault(dep => dep.Id == g.Key);
                    var deptName = department?.Name ?? deptDevices.First().Department;

                    return new DepartmentCostItem
                    {
                        DepartmentName = deptName,
                        MaintenanceCost = deptRecords.Sum(m => m.LaborCost),
                        PartsCost = deptRecords.Sum(m => m.PartsCost),
                        DeviceCount = deptDevices.Count
                    };
                })
                .OrderByDescending(item => item.TotalCost)
                .ToList();

            return departmentCosts;
        }

        public async Task<List<MonthlyIncidentItem>> GetRiskIncidentTrendAsync(int months = 12)
        {
            var incidents = await _unitOfWork.RiskIncidents.GetAllAsync();
            var cutoffDate = DateTime.Now.AddMonths(-months);

            var trendData = new List<MonthlyIncidentItem>();

            for (int i = months - 1; i >= 0; i--)
            {
                var month = DateTime.Now.AddMonths(-i);
                var monthStart = new DateTime(month.Year, month.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                var monthIncidents = incidents.Count(inc =>
                    inc.IncidentDate >= monthStart && inc.IncidentDate <= monthEnd);

                trendData.Add(new MonthlyIncidentItem
                {
                    Month = monthStart.ToString("MMM yyyy"),
                    IncidentCount = monthIncidents
                });
            }

            return trendData;
        }

        public async Task<List<FailingEquipmentItem>> GetTopFailingEquipmentAsync(int count = 10, DateTime? startDate = null, DateTime? endDate = null)
        {
            var maintenanceRecords = await _unitOfWork.MaintenanceRecords.GetAllAsync();
            var devices = await _unitOfWork.MedicalDevices.GetAllAsync();

            var records = maintenanceRecords.ToList();
            if (startDate.HasValue)
                records = records.Where(m => m.ScheduledDate >= startDate.Value).ToList();
            if (endDate.HasValue)
                records = records.Where(m => m.ScheduledDate <= endDate.Value).ToList();

            var deviceGroups = records
                .GroupBy(m => m.DeviceId)
                .Select(g => new
                {
                    DeviceId = g.Key,
                    MaintenanceCount = g.Count(),
                    TotalCost = g.Sum(m => m.TotalCost),
                    TotalDowntime = g.Sum(m => (double)(m.DowntimeHours ?? 0))
                })
                .OrderByDescending(x => x.MaintenanceCount)
                .Take(count)
                .ToList();

            var result = new List<FailingEquipmentItem>();
            foreach (var item in deviceGroups)
            {
                var device = devices.FirstOrDefault(d => d.Id == item.DeviceId);
                if (device != null)
                {
                    result.Add(new FailingEquipmentItem
                    {
                        DeviceId = device.Id,
                        DeviceName = device.DeviceName,
                        DeviceCode = device.DeviceCode,
                        Category = device.Category,
                        Department = device.Department,
                        MaintenanceCount = item.MaintenanceCount,
                        TotalCost = item.TotalCost,
                        DowntimeHours = item.TotalDowntime
                    });
                }
            }

            return result;
        }

        public async Task<List<WarrantyExpiryItem>> GetWarrantyExpiryTimelineAsync()
        {
            var devices = await _unitOfWork.MedicalDevices.GetAllAsync();
            var activeDevices = devices.Where(d => d.IsActive && d.WarrantyExpiryDate > DateTime.Now).ToList();

            var now = DateTime.Now;
            var quarters = new Dictionary<string, (DateTime Start, DateTime End)>();

            // Build quarters for the next 12 months
            for (int i = 0; i < 4; i++)
            {
                var quarterStart = new DateTime(now.Year, ((now.Month - 1) / 3) * 3 + 1, 1).AddMonths(i * 3);
                var quarterEnd = quarterStart.AddMonths(3).AddDays(-1);
                var quarterLabel = $"Q{((quarterStart.Month - 1) / 3) + 1} {quarterStart.Year}";
                quarters[quarterLabel] = (quarterStart, quarterEnd);
            }

            var result = new List<WarrantyExpiryItem>();

            foreach (var quarter in quarters)
            {
                var expiringDevices = activeDevices
                    .Where(d => d.WarrantyExpiryDate >= quarter.Value.Start && d.WarrantyExpiryDate <= quarter.Value.End)
                    .ToList();

                var item = new WarrantyExpiryItem
                {
                    Quarter = quarter.Key,
                    DeviceCount = expiringDevices.Count,
                    Devices = expiringDevices.Select(d => new WarrantyDeviceItem
                    {
                        DeviceName = d.DeviceName,
                        DeviceCode = d.DeviceCode,
                        WarrantyExpiryDate = d.WarrantyExpiryDate,
                        Department = d.Department
                    }).ToList()
                };

                result.Add(item);
            }

            return result;
        }
    }
}
