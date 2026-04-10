using System;
using System.Collections.Generic;

namespace MomenMedmSys.Core.Entities
{
    /// <summary>
    /// Procurement request entity - needs planning, gap analysis, and procurement tracking
    /// </summary>
    public class ProcurementRequest : BaseEntity
    {
        public string RequestNumber { get; set; } = string.Empty;
        public string RequestedBy { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public string Department { get; set; } = string.Empty;

        // Needs Analysis
        public string Justification { get; set; } = string.Empty;
        public string GapAnalysis { get; set; } = string.Empty; // What gap this equipment fills
        public string ClinicalNeed { get; set; } = string.Empty;
        public int EstimatedPatientVolume { get; set; } // Expected usage

        // Specifications
        public string EquipmentType { get; set; } = string.Empty;
        public string TechnicalSpecifications { get; set; } = string.Empty;
        public string MinimumRequirements { get; set; } = string.Empty;
        public string PreferredBrands { get; set; } = string.Empty; // Comma-separated

        // Financial
        public decimal BudgetEstimate { get; set; }
        public decimal BudgetApproved { get; set; }
        public string BudgetSource { get; set; } = string.Empty;
        public string FundingSource { get; set; } = string.Empty;

        // Approval
        public ProcurementStatus Status { get; set; } = ProcurementStatus.Draft;
        public string ApprovedBy { get; set; } = string.Empty;
        public DateTime? ApprovalDate { get; set; }
        public string ApprovalNotes { get; set; } = string.Empty;

        // Supplier Selection
        public int? SelectedSupplierId { get; set; }
        public Supplier? SelectedSupplier { get; set; }
        public string SelectionJustification { get; set; } = string.Empty;

        // Order Details
        public string PurchaseOrderNumber { get; set; } = string.Empty;
        public DateTime? OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }

        // Installation
        public DateTime? InstallationDate { get; set; }
        public int? CreatedDeviceId { get; set; } // If device was created after delivery
        public MedicalDevice? CreatedDevice { get; set; }

        // Evaluation
        public ICollection<TechnicalEvaluation> TechnicalEvaluations { get; set; } = new List<TechnicalEvaluation>();

        public string Notes { get; set; } = string.Empty;
    }

    /// <summary>
    /// Technical evaluation of supplier offers during procurement
    /// </summary>
    public class TechnicalEvaluation : BaseEntity
    {
        public int ProcurementRequestId { get; set; }
        public ProcurementRequest ProcurementRequest { get; set; } = null!;

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;

        // Evaluation Criteria (1-10 scale)
        public int TechnicalScore { get; set; }
        public int FinancialScore { get; set; }
        public int QualityScore { get; set; }
        public int SupportScore { get; set; }
        public int DeliveryScore { get; set; }
        public decimal OverallScore => (TechnicalScore + FinancialScore + QualityScore + SupportScore + DeliveryScore) / 5.0m;

        // Financial
        public decimal QuotedPrice { get; set; }
        public decimal TotalCostOfOwnership { get; set; } // 5-year cost estimate
        public string PaymentTerms { get; set; } = string.Empty;
        public int WarrantyYears { get; set; }

        // Technical
        public string TechnicalCompliance { get; set; } = string.Empty;
        public string Deviations { get; set; } = string.Empty;
        public string DeliveryTimeframe { get; set; } = string.Empty;

        // Decision
        public bool IsSelected { get; set; }
        public string EvaluationNotes { get; set; } = string.Empty;
        public string EvaluatedBy { get; set; } = string.Empty;
        public DateTime EvaluationDate { get; set; }
    }

    public enum ProcurementStatus
    {
        Draft = 1,
        Submitted = 2,
        UnderReview = 3,
        Approved = 4,
        Rejected = 5,
        QuotationRequested = 6,
        Evaluating = 7,
        Ordered = 8,
        Delivered = 9,
        Installed = 10,
        Cancelled = 11
    }
}
