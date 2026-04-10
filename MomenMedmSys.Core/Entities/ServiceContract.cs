using System;
using System.Collections.Generic;

namespace MomenMedmSys.Core.Entities
{
    /// <summary>
    /// Service contract management for external maintenance
    /// </summary>
    public class ServiceContract : BaseEntity
    {
        public string ContractNumber { get; set; } = string.Empty;
        public string ContractName { get; set; } = string.Empty;

        // Provider
        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
        public string Provider { get; set; } = string.Empty; // Legacy
        public string ContactPerson { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;

        // Contract Period
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AutoRenew { get; set; }
        public DateTime? RenewalNoticeDate { get; set; }
        public int RenewalNoticeDays { get; set; } = 30;

        // Coverage
        public string CoverageDescription { get; set; } = string.Empty;
        public decimal ContractValue { get; set; }
        public string PaymentTerms { get; set; } = string.Empty;

        // Covered Devices
        public string CoveredDeviceCategories { get; set; } = string.Empty; // JSON or comma-separated
        public ICollection<int> CoveredDeviceIds { get; set; } = new List<int>();

        // SLA
        public int ResponseTimeHours { get; set; }
        public int ResolutionTimeHours { get; set; }
        public string SLADetails { get; set; } = string.Empty;
        public string PenaltyClause { get; set; } = string.Empty;

        // Performance
        public int TotalCalls { get; set; }
        public int CompletedCalls { get; set; }
        public decimal SatisfactionScore { get; set; }

        // Documents
        public string ContractFilePath { get; set; } = string.Empty;
        public string WarrantyCertificatePath { get; set; } = string.Empty;
        public string Attachments { get; set; } = string.Empty;

        public ContractStatus Status { get; set; } = ContractStatus.Active;

        public bool IsExpiringSoon => EndDate <= DateTime.Now.AddDays(RenewalNoticeDays);
        public bool IsExpired => EndDate < DateTime.Now && !AutoRenew;
    }

    public enum ContractStatus
    {
        Draft,
        Active,
        Expired,
        Terminated,
        PendingRenewal
    }
}
