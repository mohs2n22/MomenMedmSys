using System;
using System.Collections.Generic;

namespace MomenMedmSys.Core.Entities
{
    /// <summary>
    /// Maintenance record for tracking preventative and corrective maintenance
    /// </summary>
    public class MaintenanceRecord : BaseEntity
    {
        public int DeviceId { get; set; }
        public MedicalDevice Device { get; set; } = null!;

        // Computed property for convenient display
        public string DeviceName => Device?.DeviceName ?? string.Empty;

        public MaintenanceType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Scheduling
        public DateTime ScheduledDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? NextDueDate { get; set; }
        public RecurrenceFrequency Recurrence { get; set; } = RecurrenceFrequency.None;
        public int RecurrenceInterval { get; set; } // e.g., every 3 months

        // Execution
        public string PerformedBy { get; set; } = string.Empty; // Technician name
        public int? PerformedByStaffId { get; set; }
        public StaffMember? PerformedByStaff { get; set; }
        public bool IsExternalContractor { get; set; }
        public string ContractorName { get; set; } = string.Empty;
        public string ContractReference { get; set; } = string.Empty;

        // Status & Results
        public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Scheduled;
        public string Findings { get; set; } = string.Empty;
        public string ActionsTaken { get; set; } = string.Empty;
        public string Recommendations { get; set; } = string.Empty;
        public string RootCause { get; set; } = string.Empty;

        // Downtime
        public decimal? DowntimeHours { get; set; }

        // Verification
        public bool VerificationPerformed { get; set; }
        public string VerifiedBy { get; set; } = string.Empty;
        public DateTime? VerificationDate { get; set; }

        // Work Order Link
        public int? WorkOrderId { get; set; }
        public WorkOrder? WorkOrder { get; set; }

        // Priority & SLA
        public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.Medium;
        public DateTime? EstimatedCompletionDate { get; set; }

        // Costs
        public decimal LaborCost { get; set; }
        public decimal PartsCost { get; set; }
        public decimal TotalCost => LaborCost + PartsCost;

        // Parts Used
        public ICollection<SparePartUsage> SparePartUsages { get; set; } = new List<SparePartUsage>();

        // Attachments
        public string Attachments { get; set; } = string.Empty; // JSON or file paths
    }

    public enum MaintenanceType
    {
        Preventive,
        Corrective,
        Emergency,
        Inspection,
        Calibration
    }

    public enum MaintenanceStatus
    {
        Scheduled,
        InProgress,
        Completed,
        Cancelled,
        Overdue
    }

    public enum RecurrenceFrequency
    {
        None,
        Daily,
        Weekly,
        Monthly,
        Quarterly,
        SemiAnnually,
        Annually
    }
}
