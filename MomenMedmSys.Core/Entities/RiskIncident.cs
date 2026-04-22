using System;
using System.Collections.Generic;

namespace MomenMedmSys.Core.Entities
{
    /// <summary>
    /// Risk and safety incident management (ISO 14971 compliance)
    /// </summary>
    public class RiskIncident : BaseEntity
    {
        public int DeviceId { get; set; }
        public MedicalDevice Device { get; set; } = null!;

        // Computed property for convenient display
        public string DeviceName => Device?.DeviceName ?? string.Empty;

        public string IncidentCode { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        // Incident Details
        public DateTime IncidentDate { get; set; }
        public string ReportedBy { get; set; } = string.Empty;
        public string IncidentLocation { get; set; } = string.Empty;
        public string IncidentType { get; set; } = string.Empty; // e.g., Malfunction, Injury, Near Miss, Recall
        
        // Risk Assessment
        public SeverityLevel Severity { get; set; } = SeverityLevel.Medium;
        public ProbabilityLevel Probability { get; set; } = ProbabilityLevel.Possible;
        public RiskLevel OverallRisk => CalculateRisk();
        
        // Impact
        public bool PatientInjury { get; set; }
        public string InjuryDescription { get; set; } = string.Empty;
        public bool StaffInjury { get; set; }
        public string StaffMembersInvolved { get; set; } = string.Empty; // Names/details
        public string PatientsInvolved { get; set; } = string.Empty; // Names/details
        public int AffectedPatients { get; set; }
        public int AffectedStaff { get; set; }
        
        // Investigation
        public string RootCause { get; set; } = string.Empty;
        public string InvestigationFindings { get; set; } = string.Empty;
        public DateTime? InvestigationCompleteDate { get; set; }
        
        // Corrective Actions
        public string CorrectiveActions { get; set; } = string.Empty;
        public string PreventiveActions { get; set; } = string.Empty;
        public DateTime? ActionDeadline { get; set; }
        public IncidentStatus Status { get; set; } = IncidentStatus.Open;
        
        // Recall Management
        public bool IsRecall { get; set; }
        public string RecallNumber { get; set; } = string.Empty;
        public string RecallAuthority { get; set; } = string.Empty; // e.g., FDA, Ministry of Health
        public DateTime? RecallDate { get; set; }
        public bool RegulatoryReported { get; set; }
        public DateTime? RegulatoryReportDate { get; set; }
        public string RegulatoryReportReference { get; set; } = string.Empty;

        // Resolution
        public string Resolution { get; set; } = string.Empty;
        public DateTime? ResolvedDate { get; set; }
        public string ResolvedBy { get; set; } = string.Empty;

        // Attachments
        public string Attachments { get; set; } = string.Empty; // Evidence, photos, documents

        private RiskLevel CalculateRisk()
        {
            int score = (int)Severity * (int)Probability;
            return score switch
            {
                <= 2 => RiskLevel.Low,
                <= 4 => RiskLevel.Medium,
                <= 6 => RiskLevel.High,
                _ => RiskLevel.Critical
            };
        }
    }

    public enum SeverityLevel
    {
        Negligible = 1,
        Minor = 2,
        Medium = 3,
        Major = 4,
        Critical = 5
    }

    public enum ProbabilityLevel
    {
        Rare = 1,
        Unlikely = 2,
        Possible = 3,
        Likely = 4,
        AlmostCertain = 5
    }

    public enum RiskLevel
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum IncidentStatus
    {
        Open,
        UnderInvestigation,
        PendingAction,
        Resolved,
        Closed
    }
}
