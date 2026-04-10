using System;
using System.Collections.Generic;

namespace MomenMedmSys.Core.Entities
{
    /// <summary>
    /// Medical Device/Equipment entity - core asset register
    /// </summary>
    public class MedicalDevice : BaseEntity
    {
        public string DeviceCode { get; set; } = string.Empty; // Unique identification number
        public string DeviceName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // e.g., Imaging, Laboratory, Surgical
        public string SubCategory { get; set; } = string.Empty;

        // Tracking
        public string Barcode { get; set; } = string.Empty; // Barcode/RFID for inventory tracking
        public string RFIDTag { get; set; } = string.Empty;
        public string AssetTagNumber { get; set; } = string.Empty; // Financial asset tracking
        public string UdiCode { get; set; } = string.Empty; // Unique Device Identification (FDA)
        public string RegulatoryClass { get; set; } = string.Empty; // FDA Class I/II/III

        // Purchase & Financial
        public DateTime PurchaseDate { get; set; }
        public decimal PurchasePrice { get; set; }
        public string AcquisitionMethod { get; set; } = string.Empty; // Purchase, Lease, Donation, Rental
        public DateTime? InstallationDate { get; set; }
        public DateTime? CommissioningDate { get; set; }
        public int? EstimatedLifespanYears { get; set; }
        public DateTime? ExpectedDisposalDate { get; set; }
        public string DepreciationMethod { get; set; } = string.Empty;
        public decimal? CurrentDepreciatedValue { get; set; }
        public decimal TotalMaintenanceCost { get; set; } // Computed total expenditure

        // Supplier & Warranty
        public int? SupplierId { get; set; }
        public Supplier? SupplierEntity { get; set; }
        public string SupplierName { get; set; } = string.Empty; // Legacy display name
        public string WarrantyProvider { get; set; } = string.Empty;
        public DateTime WarrantyExpiryDate { get; set; }
        public string WarrantyCertificatePath { get; set; } = string.Empty;
        public string WarrantyTerms { get; set; } = string.Empty;

        // Location & Assignment
        public int? DepartmentId { get; set; }
        public Department? DepartmentEntity { get; set; }
        public string Department { get; set; } = string.Empty; // Legacy
        public string Building { get; set; } = string.Empty;
        public string Floor { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;

        // GPS Location
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        // Staff Assignment
        public int? AssignedStaffId { get; set; }
        public StaffMember? AssignedStaff { get; set; }
        public string AssignedTo { get; set; } = string.Empty; // Legacy

        // Technical Specifications
        public string TechnicalSpecs { get; set; } = string.Empty;
        public string PowerRequirements { get; set; } = string.Empty;
        public string SoftwareVersion { get; set; } = string.Empty;
        public bool NetworkConnected { get; set; }
        public bool RequiresCalibration { get; set; }
        public bool RequiresPreventiveMaintenance { get; set; }
        public bool RequiresElectricalSafetyTesting { get; set; }

        // Manuals & Documents
        public string OperationalManualPath { get; set; } = string.Empty;
        public string MaintenanceManualPath { get; set; } = string.Empty;

        // Lifecycle Status
        public DeviceStatus Status { get; set; } = DeviceStatus.Active;
        public DateTime? DisposalDate { get; set; }
        public string DisposalReason { get; set; } = string.Empty;

        // Risk Classification
        public RiskClass RiskClassification { get; set; } = RiskClass.Medium;
        public string SafetyNotes { get; set; } = string.Empty;

        // Quick Reference Dates
        public DateTime? LastMaintenanceDate { get; set; }
        public DateTime? LastCalibrationDate { get; set; }
        public DateTime? LastSafetyTestDate { get; set; }

        // Navigation Properties
        public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();
        public ICollection<CalibrationRecord> CalibrationRecords { get; set; } = new List<CalibrationRecord>();
        public ICollection<RiskIncident> RiskIncidents { get; set; } = new List<RiskIncident>();
        public ICollection<SparePart> SpareParts { get; set; } = new List<SparePart>();
        public ICollection<DeviceDocument> Documents { get; set; } = new List<DeviceDocument>();
        public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
        public ICollection<ElectricalSafetyTest> SafetyTests { get; set; } = new List<ElectricalSafetyTest>();
        public ICollection<TrainingRecord> TrainingRecords { get; set; } = new List<TrainingRecord>();
        public ICollection<AssignedDevice> AssignedDevices { get; set; } = new List<AssignedDevice>();
    }

    public enum DeviceStatus
    {
        Active,
        UnderMaintenance,
        OutOfService,
        PendingCalibration,
        Decommissioned,
        Disposed
    }

    public enum RiskClass
    {
        Low,
        Medium,
        High,
        Critical
    }
}
