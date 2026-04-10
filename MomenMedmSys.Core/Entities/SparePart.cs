using System;
using System.Collections.Generic;

namespace MomenMedmSys.Core.Entities
{
    /// <summary>
    /// Spare parts and inventory management
    /// </summary>
    public class SparePart : BaseEntity
    {
        public string PartNumber { get; set; } = string.Empty;
        public string PartName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Tracking
        public string Barcode { get; set; } = string.Empty;

        // Compatibility
        public int? DeviceId { get; set; }
        public MedicalDevice? Device { get; set; }
        public string CompatibleModels { get; set; } = string.Empty;

        // Inventory
        public string Category { get; set; } = string.Empty;
        public int? SupplierId { get; set; }
        public Supplier? SupplierEntity { get; set; }
        public string SupplierName { get; set; } = string.Empty; // Legacy display name
        public string Manufacturer { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int MinimumStock { get; set; }
        public int MaximumStock { get; set; }
        public int ReorderPoint { get; set; } // Auto-reorder trigger
        public decimal UnitCost { get; set; }
        public string StorageLocation { get; set; } = string.Empty;

        // Procurement
        public DateTime? LastOrderDate { get; set; }
        public decimal LastOrderCost { get; set; }
        public int LeadTimeDays { get; set; } // Expected delivery time

        // Status
        public bool IsCritical { get; set; } // Essential for operation
        public bool IsObsolete { get; set; }

        // Usage Analytics
        public DateTime? LastUsedDate { get; set; }
        public int TotalUsageCount { get; set; }
        public decimal TotalUsageValue => TotalUsageCount * UnitCost;

        // Stock alerts
        public bool IsLowStock => CurrentStock <= MinimumStock;
        public bool NeedsReorder => CurrentStock <= ReorderPoint;

        // Navigation
        public ICollection<SparePartUsage> Usages { get; set; } = new List<SparePartUsage>();
    }

    /// <summary>
    /// Tracks spare part usage in maintenance activities
    /// </summary>
    public class SparePartUsage
    {
        public int Id { get; set; }
        public int SparePartId { get; set; }
        public SparePart SparePart { get; set; } = null!;
        
        public int MaintenanceRecordId { get; set; }
        public MaintenanceRecord MaintenanceRecord { get; set; } = null!;
        
        public int QuantityUsed { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime UsedDate { get; set; } = DateTime.Now;
    }
}
