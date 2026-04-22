using System;
using System.Collections.Generic;

namespace MomenMedmSys.Core.Entities
{
    /// <summary>
    /// Work order entity - tracks fault reports and repair requests from request to completion
    /// </summary>
    public class WorkOrder : BaseEntity
    {
        public string WorkOrderNumber { get; set; } = string.Empty;
        public int DeviceId { get; set; }
        public MedicalDevice Device { get; set; } = null!;

        // Computed property for convenient display
        public string DeviceName => Device?.DeviceName ?? string.Empty;

        // Request Details
        public string ReportedBy { get; set; } = string.Empty;
        public DateTime ReportDate { get; set; } = DateTime.Now;
        public string FaultDescription { get; set; } = string.Empty;
        public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.Medium;
        public string DeviceCategory { get; set; } = string.Empty; // For reporting

        // Assignment
        public string AssignedTo { get; set; } = string.Empty;
        public DateTime? AssignedDate { get; set; }
        public bool IsExternalContractor { get; set; }
        public string ContractorName { get; set; } = string.Empty;

        // Scheduling
        public DateTime? ScheduledStartDate { get; set; }
        public DateTime? ScheduledEndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        // Status
        public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Open;
        public string ResolutionDescription { get; set; } = string.Empty;
        public string RootCause { get; set; } = string.Empty;

        // SLA Tracking
        public int? ResponseTimeHours { get; set; } // Time from report to assignment
        public int? ResolutionTimeHours { get; set; } // Time from report to completion
        public DateTime? SLADeadline { get; set; }
        public bool IsSLABreached => SLADeadline.HasValue && CompletedDate > SLADeadline;

        // Costs
        public decimal EstimatedCost { get; set; }
        public decimal ActualCost { get; set; }

        // Link to maintenance record (if converted)
        public int? MaintenanceRecordId { get; set; }
        public MaintenanceRecord? MaintenanceRecord { get; set; }

        // Attachments
        public string Attachments { get; set; } = string.Empty; // JSON or file paths
        public string Notes { get; set; } = string.Empty;
    }

    public enum WorkOrderPriority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4,
        Emergency = 5
    }

    public enum WorkOrderStatus
    {
        Open = 1,
        Assigned = 2,
        InProgress = 3,
        PendingParts = 4,
        Completed = 5,
        Cancelled = 6,
        OnHold = 7
    }
}
