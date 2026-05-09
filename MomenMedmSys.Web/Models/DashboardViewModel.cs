using System.Collections.Generic;

namespace MomenMedmSys.Web.Models
{
    public class DashboardViewModel
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
        public List<string> Alerts { get; set; } = new List<string>();
        public List<string> WarrantyExpiryAlerts { get; set; } = new List<string>();
    }
}
