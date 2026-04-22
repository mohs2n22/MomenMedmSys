using System;
using System.Threading.Tasks;
using MomenMedmSys.Core.Entities;

namespace MomenMedmSys.Services
{
    public class LicenseResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public LicenseInfo? License { get; set; }
    }

    /// <summary>
    /// Service for license activation and validation — license key generation (3-month, 1-year, lifetime),
    /// key format validation, hardware fingerprinting, device registration, and license status reporting.
    /// </summary>
    public interface ILicenseService
    {
        Task<LicenseInfo?> GetCurrentLicenseAsync();
        Task<LicenseResult> ActivateAsync(string licenseKey);
        Task<LicenseResult> ValidateAsync();
        Task<LicenseResult> RegisterCurrentDeviceAsync();
        string GenerateLicenseKey(LicenseType type);
        bool ValidateKeyFormat(string key);
        Task<LicenseResult> RemoveDeviceAsync(int licenseDeviceId);
        Task<int> GetRemainingSlotsAsync();
        Task<bool> IsLifetimeLicenseAsync();
        Task<string> GetLicenseStatusTextAsync();
        Task<LicenseResult> UpdateHospitalInfoAsync(string hospitalName, string administratorName, string licenseNumber);
        Task<LicenseResult> GenerateLicenseFileAsync(string outputPath, LicenseType licenseType, string hospitalName, string administratorName, string licenseNumber);
        Task<LicenseResult> UpdateSystemExecutableAsync(string outputPath, LicenseType licenseType, string hospitalName, string administratorName, string licenseNumber);
    }
}
