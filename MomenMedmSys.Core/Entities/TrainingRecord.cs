using System;

namespace MomenMedmSys.Core.Entities
{
    /// <summary>
    /// Training record entity - tracks staff qualifications for specific devices
    /// </summary>
    public class TrainingRecord : BaseEntity
    {
        public int StaffMemberId { get; set; }
        public StaffMember StaffMember { get; set; } = null!;

        public int? DeviceId { get; set; }
        public MedicalDevice? Device { get; set; }

        public string DeviceCategory { get; set; } = string.Empty; // If training applies to a category, not specific device

        // Training Details
        public string TrainingTitle { get; set; } = string.Empty;
        public string TrainingDescription { get; set; } = string.Empty;
        public DateTime TrainingDate { get; set; }
        public DateTime? ExpiryDate { get; set; } // Certifications may expire
        public string Trainer { get; set; } = string.Empty;
        public string TrainingProvider { get; set; } = string.Empty; // Internal, External, Manufacturer
        public string CertificationNumber { get; set; } = string.Empty;
        public TrainingStatus Status { get; set; } = TrainingStatus.Active;

        // Assessment
        public bool AssessmentPassed { get; set; }
        public decimal? AssessmentScore { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string AttachmentPath { get; set; } = string.Empty; // Certificate file

        public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.Now;
        public bool IsExpiringSoon
        {
            get
            {
                if (!ExpiryDate.HasValue) return false;
                return ExpiryDate.Value <= DateTime.Now.AddDays(30);
            }
        }
    }

    public enum TrainingStatus
    {
        Active,
        Expired,
        Pending,
        Cancelled
    }
}
