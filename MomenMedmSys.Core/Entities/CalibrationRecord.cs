using System;

namespace MomenMedmSys.Core.Entities
{
    /// <summary>
    /// Calibration and verification record for medical devices
    /// </summary>
    public class CalibrationRecord : BaseEntity
    {
        public int DeviceId { get; set; }
        public MedicalDevice Device { get; set; } = null!;

        public string CalibrationType { get; set; } = string.Empty; // e.g., Full Calibration, Verification, Spot Check
        public string StandardUsed { get; set; } = string.Empty; // Reference standard/equipment used
        public string StandardCertificate { get; set; } = string.Empty;

        // Calibration Details
        public DateTime CalibrationDate { get; set; }
        public DateTime NextDueDate { get; set; }
        public string PerformedBy { get; set; } = string.Empty;
        public int? PerformedByStaffId { get; set; }
        public StaffMember? PerformedByStaff { get; set; }
        public bool IsExternalLab { get; set; }
        public string LaboratoryName { get; set; } = string.Empty;
        public string AccreditationNumber { get; set; } = string.Empty;

        // Results
        public CalibrationResult Result { get; set; } = CalibrationResult.Pass;
        public string AsFoundData { get; set; } = string.Empty; // Readings before calibration
        public string AsLeftData { get; set; } = string.Empty; // Readings after calibration
        public string ToleranceLimits { get; set; } = string.Empty;
        public string MeasurementUncertainty { get; set; } = string.Empty;

        // Environmental Conditions
        public decimal? Temperature { get; set; } // °C during calibration
        public decimal? Humidity { get; set; } // %RH during calibration
        public string EnvironmentalConditions { get; set; } = string.Empty;

        // Procedure
        public string CalibrationProcedure { get; set; } = string.Empty;

        // Adjustments
        public bool AdjustmentsMade { get; set; }
        public string AdjustmentDescription { get; set; } = string.Empty;

        // Certification
        public string CertificateNumber { get; set; } = string.Empty;
        public string TechnicianSignature { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;

        // Attachments
        public string Attachments { get; set; } = string.Empty;

        // Link to electrical safety test (if done together)
        public int? ElectricalSafetyTestId { get; set; }
        public ElectricalSafetyTest? ElectricalSafetyTest { get; set; }
    }

    public enum CalibrationResult
    {
        Pass,
        Fail,
        PassWithAdjustment,
        OutOfTolerance,
        NotCalibrated
    }
}
