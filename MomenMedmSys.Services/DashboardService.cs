using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Data;

namespace MomenMedmSys.Services
{
    public interface IDashboardService
    {
        Task<DashboardStats> GetDashboardStatsAsync();
    }

    public class DashboardStats
    {
        // Device Stats
        public int TotalDevices { get; set; }
        public int ActiveDevices { get; set; }
        public int UnderMaintenanceDevices { get; set; }
        public int OutOfServiceDevices { get; set; }

        // Maintenance Stats
        public int ScheduledMaintenanceCount { get; set; }
        public int OverdueMaintenanceCount { get; set; }

        // Calibration Stats
        public int OverdueCalibrationCount { get; set; }
        public int UpcomingCalibrationCount { get; set; }

        // Risk Stats
        public int OpenIncidentsCount { get; set; }
        public int CriticalIncidentsCount { get; set; }

        // Work Order Stats
        public int OpenWorkOrdersCount { get; set; }
        public int OverdueWorkOrdersCount { get; set; }

        // Spare Parts Stats
        public int LowStockPartsCount { get; set; }
        public decimal TotalInventoryValue { get; set; }

        // Financial
        public decimal TotalAssetValue { get; set; }
        public decimal TotalMaintenanceCost { get; set; }

        // Alerts
        public List<string> Alerts { get; set; } = new();

        // Warranty Expiry Alerts
        public List<string> WarrantyExpiryAlerts { get; set; } = new();
    }

    public class DashboardService : IDashboardService
    {
        private readonly IDeviceService _deviceService;
        private readonly IMaintenanceService _maintenanceService;
        private readonly ICalibrationService _calibrationService;
        private readonly IRiskService _riskService;
        private readonly IWorkOrderService _workOrderService;
        private readonly ISparePartService _sparePartService;

        public DashboardService(
            IDeviceService deviceService,
            IMaintenanceService maintenanceService,
            ICalibrationService calibrationService,
            IRiskService riskService,
            IWorkOrderService workOrderService,
            ISparePartService sparePartService)
        {
            _deviceService = deviceService;
            _maintenanceService = maintenanceService;
            _calibrationService = calibrationService;
            _riskService = riskService;
            _workOrderService = workOrderService;
            _sparePartService = sparePartService;
        }

        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            var stats = new DashboardStats();

            // Device stats
            stats.TotalDevices = await _deviceService.GetTotalDeviceCountAsync();
            stats.ActiveDevices = await _deviceService.GetActiveDeviceCountAsync();
            var underMaintenance = await _deviceService.GetDevicesByStatusAsync(DeviceStatus.UnderMaintenance);
            stats.UnderMaintenanceDevices = underMaintenance.Count();
            var outOfService = await _deviceService.GetDevicesByStatusAsync(DeviceStatus.OutOfService);
            stats.OutOfServiceDevices = outOfService.Count();

            // Maintenance stats
            stats.ScheduledMaintenanceCount = await _maintenanceService.GetScheduledCountAsync();
            stats.OverdueMaintenanceCount = await _maintenanceService.GetOverdueCountAsync();

            // Calibration stats
            stats.OverdueCalibrationCount = await _calibrationService.GetOverdueCountAsync();
            var upcomingCalibrations = await _calibrationService.GetUpcomingCalibrationsAsync();
            stats.UpcomingCalibrationCount = upcomingCalibrations.Count();

            // Risk stats
            stats.OpenIncidentsCount = await _riskService.GetOpenIncidentCountAsync();
            stats.CriticalIncidentsCount = await _riskService.GetCriticalIncidentCountAsync();

            // Work order stats
            stats.OpenWorkOrdersCount = await _workOrderService.GetOpenWorkOrderCountAsync();
            stats.OverdueWorkOrdersCount = await _workOrderService.GetOverdueWorkOrderCountAsync();

            // Spare parts stats
            stats.LowStockPartsCount = await _sparePartService.GetLowStockCountAsync();
            stats.TotalInventoryValue = await _sparePartService.GetTotalInventoryValueAsync();

            // Financial
            stats.TotalAssetValue = await _deviceService.GetTotalAssetValueAsync();

            // Generate alerts
            if (stats.OverdueMaintenanceCount > 0)
                stats.Alerts.Add($"{stats.OverdueMaintenanceCount} overdue maintenance task(s)");

            if (stats.OverdueCalibrationCount > 0)
                stats.Alerts.Add($"{stats.OverdueCalibrationCount} overdue calibration(s)");

            if (stats.OpenIncidentsCount > 0)
                stats.Alerts.Add($"{stats.OpenIncidentsCount} open incident(s) require attention");

            if (stats.CriticalIncidentsCount > 0)
                stats.Alerts.Add($"{stats.CriticalIncidentsCount} CRITICAL incident(s)");

            if (stats.OverdueWorkOrdersCount > 0)
                stats.Alerts.Add($"{stats.OverdueWorkOrdersCount} overdue work order(s)");

            if (stats.LowStockPartsCount > 0)
                stats.Alerts.Add($"{stats.LowStockPartsCount} spare part(s) below minimum stock");

            // Warranty expiry alerts
            var expiringWarranties = await _deviceService.GetDevicesWithExpiringWarrantyAsync(30);
            foreach (var device in expiringWarranties)
            {
                stats.WarrantyExpiryAlerts.Add($"{device.DeviceName} ({device.DeviceCode}) - Warranty expires {device.WarrantyExpiryDate:yyyy-MM-dd}");
            }

            return stats;
        }
    }
}
