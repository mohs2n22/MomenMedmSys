using System;

namespace MomenMedmSys.Core.Entities
{
    /// <summary>
    /// Device document entity - digital device file for manuals, certificates, and documents
    /// </summary>
    public class DeviceDocument : BaseEntity
    {
        public int DeviceId { get; set; }
        public MedicalDevice Device { get; set; } = null!;

        public DocumentType DocumentType { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; } // Bytes
        public string MimeType { get; set; } = string.Empty; // e.g., application/pdf
        public DateTime UploadDate { get; set; } = DateTime.Now;
        public string UploadedBy { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public DateTime? ExpiryDate { get; set; } // For warranties, certifications
        public bool IsCurrentVersion { get; set; } = true;

        public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.Now;
        public bool IsExpiringSoon
        {
            get
            {
                if (!ExpiryDate.HasValue) return false;
                return ExpiryDate.Value <= DateTime.Now.AddDays(30);
            }
        }

        public string FileSizeDisplay
        {
            get
            {
                if (FileSize < 1024) return $"{FileSize} B";
                if (FileSize < 1024 * 1024) return $"{FileSize / 1024.0:F1} KB";
                return $"{FileSize / (1024.0 * 1024.0):F1} MB";
            }
        }
    }

    public enum DocumentType
    {
        OperationManual = 1,
        MaintenanceManual = 2,
        WarrantyCertificate = 3,
        TrainingMaterial = 4,
        RegulatoryCertificate = 5,
        CalibrationCertificate = 6,
        SafetyTestReport = 7,
        TechnicalDrawing = 8,
        SoftwareManual = 9,
        Other = 99
    }
}
