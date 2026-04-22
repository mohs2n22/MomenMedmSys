using System;
using System.Collections.Generic;

namespace MomenMedmSys.Core.Entities
{
    public class LicenseInfo : BaseEntity
    {
        public string LicenseKey { get; set; } = string.Empty;
        public LicenseType LicenseType { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string PrimaryMacAddress { get; set; } = string.Empty;
        public string HardwareFingerprint { get; set; } = string.Empty;
        public int MaxDevices { get; set; } = 1;
        public int RegisteredDeviceCount { get; set; }
        public bool IsActivated { get; set; }

        // Hospital / Institution Identification
        public string HospitalName { get; set; } = string.Empty;
        public string AdministratorName { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;

        public ICollection<LicenseDevice> LicensedDevices { get; set; } = new List<LicenseDevice>();
    }

    public class LicenseDevice : BaseEntity
    {
        public int LicenseInfoId { get; set; }
        public LicenseInfo License { get; set; } = null!;
        public string MacAddress { get; set; } = string.Empty;
        public string HardwareFingerprint { get; set; } = string.Empty;
        public string MachineName { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; } = DateTime.Now;
    }
}
