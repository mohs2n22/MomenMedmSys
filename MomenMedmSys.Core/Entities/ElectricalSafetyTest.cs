using System;

namespace MomenMedmSys.Core.Entities
{
    /// <summary>
    /// Electrical safety test entity - documents safety testing for patient and staff protection
    /// </summary>
    public class ElectricalSafetyTest : BaseEntity
    {
        public int DeviceId { get; set; }
        public MedicalDevice Device { get; set; } = null!;

        // Test Details
        public DateTime TestDate { get; set; }
        public DateTime NextDueDate { get; set; }
        public SafetyTestType TestType { get; set; } = SafetyTestType.FullSafetyTest;
        public string TestStandard { get; set; } = string.Empty; // e.g., IEC 60601-1, NFPA 99

        // Test Equipment
        public string TestEquipmentUsed { get; set; } = string.Empty;
        public string TestEquipmentCalibration { get; set; } = string.Empty; // Calibration cert of test equipment

        // Measurements
        public decimal? EarthResistanceMeasured { get; set; } // Ohms
        public decimal EarthResistanceLimit { get; set; } // e.g., 0.2 ohms
        public bool EarthResistancePass => !EarthResistanceMeasured.HasValue || EarthResistanceMeasured.Value <= EarthResistanceLimit;

        public decimal? LeakageCurrentMeasured { get; set; } // mA
        public decimal LeakageCurrentLimit { get; set; } // e.g., 0.1 mA (100 µA)
        public bool LeakageCurrentPass => !LeakageCurrentMeasured.HasValue || LeakageCurrentMeasured.Value <= LeakageCurrentLimit;

        public decimal? InsulationResistanceMeasured { get; set; } // MΩ
        public decimal InsulationResistanceLimit { get; set; } // e.g., 2 MΩ minimum
        public bool InsulationResistancePass => !InsulationResistanceMeasured.HasValue || InsulationResistanceMeasured.Value >= InsulationResistanceLimit;

        public decimal? TouchCurrentMeasured { get; set; } // mA
        public decimal TouchCurrentLimit { get; set; } // e.g., 0.1 mA
        public bool TouchCurrentPass => !TouchCurrentMeasured.HasValue || TouchCurrentMeasured.Value <= TouchCurrentLimit;

        // Visual Inspection
        public bool VisualInspectionPass { get; set; } = true;
        public string VisualInspectionNotes { get; set; } = string.Empty;

        // Overall Result
        public SafetyTestResult OverallResult { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public string CertificateNumber { get; set; } = string.Empty;

        // Technician
        public string PerformedBy { get; set; } = string.Empty;
        public string TechnicianSignature { get; set; } = string.Empty;
        public bool IsExternalTester { get; set; }
        public string TestingCompany { get; set; } = string.Empty;

        // Attachments
        public string Attachments { get; set; } = string.Empty;

        public bool IsOverdue => NextDueDate < DateTime.Now && OverallResult != SafetyTestResult.Fail;

        public bool IsDueSoon
        {
            get
            {
                if (OverallResult == SafetyTestResult.Fail) return false;
                return NextDueDate <= DateTime.Now.AddDays(30);
            }
        }
    }

    public enum SafetyTestType
    {
        EarthResistance = 1,
        LeakageCurrent = 2,
        InsulationResistance = 3,
        TouchCurrent = 4,
        FullSafetyTest = 5,
        VisualInspection = 6
    }

    public enum SafetyTestResult
    {
        Pass = 1,
        PassWithRemarks = 2,
        Fail = 3,
        NotTested = 4
    }
}
