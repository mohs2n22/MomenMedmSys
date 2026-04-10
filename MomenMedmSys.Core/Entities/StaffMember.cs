using System;
using System.Collections.Generic;

namespace MomenMedmSys.Core.Entities
{
    /// <summary>
    /// Staff member entity - tracks hospital staff and their qualifications
    /// </summary>
    public class StaffMember : BaseEntity
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public StaffRole Role { get; set; } = StaffRole.Staff;
        public string SubRole { get; set; } = string.Empty; // e.g., "Hardware Technician", "Biomedical Engineer"
        public string Specialization { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }

        // Account & Access Control
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsActiveAccount { get; set; } = true;
        public DateTime? LastLoginDate { get; set; }
        public int FailedLoginAttempts { get; set; }
        public bool IsLocked { get; set; }

        // Certifications
        public string Certifications { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public DateTime? LicenseExpiryDate { get; set; }

        // Permissions flags
        public bool CanManageDevices { get; set; }
        public bool CanManageMaintenance { get; set; }
        public bool CanManageCalibration { get; set; }
        public bool CanManageSpareParts { get; set; }
        public bool CanViewReports { get; set; }
        public bool CanManageNetworkDevices { get; set; }
        public bool CanManageStaff { get; set; }
        public bool CanAccessAdminPanel { get; set; }

        // Navigation
        public int? DepartmentId { get; set; }
        public Department? DepartmentEntity { get; set; }
        public ICollection<TrainingRecord> TrainingRecords { get; set; } = new List<TrainingRecord>();
        public ICollection<AssignedDevice> AssignedDevices { get; set; } = new List<AssignedDevice>();
    }

    /// <summary>
    /// Tracks which devices are assigned to which staff members
    /// </summary>
    public class AssignedDevice : BaseEntity
    {
        public int StaffMemberId { get; set; }
        public StaffMember StaffMember { get; set; } = null!;

        public int DeviceId { get; set; }
        public MedicalDevice Device { get; set; } = null!;

        public DateTime AssignmentDate { get; set; } = DateTime.Now;
        public DateTime? ReturnDate;
        public string AssignmentNotes { get; set; } = string.Empty;
    }

    public enum StaffRole
    {
        Administrator = 1,
        Physician = 2,
        Nurse = 3,
        HardwareTechnician = 4,
        ReportWriter = 5,
        BiomedicalEngineer = 6,
        LabTechnician = 7,
        Radiologist = 8,
        Receptionist = 9,
        ITSupport = 10,
        Staff = 99
    }
}
