using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Data;
using MomenMedmSys.Data.Repositories;

namespace MomenMedmSys.Services
{
    public class LicenseService : ILicenseService
    {
        private readonly MedMsysDbContext _context;
        private readonly IHardwareInfoService _hardwareInfo;
        private readonly IRepository<LicenseInfo> _licenseRepo;
        private readonly IRepository<LicenseDevice> _deviceRepo;

        public LicenseService(MedMsysDbContext context, IHardwareInfoService hardwareInfo,
            IRepository<LicenseInfo> licenseRepo, IRepository<LicenseDevice> deviceRepo)
        {
            _context = context;
            _hardwareInfo = hardwareInfo;
            _licenseRepo = licenseRepo;
            _deviceRepo = deviceRepo;
        }

        public async Task<LicenseInfo?> GetCurrentLicenseAsync() =>
            await _context.Licenses.Include(l => l.LicensedDevices).FirstOrDefaultAsync();

        public async Task<LicenseResult> ActivateAsync(string licenseKey)
        {
            if (!ValidateKeyFormat(licenseKey))
                return new LicenseResult { Success = false, Message = "Invalid license key format. Expected: MEMDS-XX-XXXX-XXXX-XXXX-XXXX" };

            var existing = await GetCurrentLicenseAsync();
            if (existing != null && existing.IsActivated)
                return new LicenseResult { Success = false, Message = "System is already activated." };

            var prefix = licenseKey.Substring(0, 8);
            LicenseType licenseType = prefix == "MEMDS-3M" ? LicenseType.ThreeMonths
                : prefix == "MEMDS-1Y" ? LicenseType.OneYear
                : prefix == "MEMDS-LT" ? LicenseType.Lifetime
                : throw new Exception("Unrecognized license type");

            int maxDevices = licenseType == LicenseType.Lifetime ? 10 : 1;

            if (!VerifyKeyChecksum(licenseKey))
                return new LicenseResult { Success = false, Message = "License key verification failed." };

            var mac = _hardwareInfo.GetMacAddress();
            var fp = _hardwareInfo.GetHardwareFingerprint();
            var machine = _hardwareInfo.GetMachineName();
            var now = DateTime.Now;
            DateTime? expiry = licenseType switch
            {
                LicenseType.ThreeMonths => now.AddMonths(3),
                LicenseType.OneYear => now.AddYears(1),
                _ => null
            };

            if (existing != null)
            {
                existing.LicenseKey = licenseKey;
                existing.LicenseType = licenseType;
                existing.ActivationDate = now;
                existing.ExpiryDate = expiry;
                existing.PrimaryMacAddress = mac;
                existing.HardwareFingerprint = fp;
                existing.MaxDevices = maxDevices;
                existing.RegisteredDeviceCount = 1;
                existing.IsActivated = true;
                existing.UpdatedAt = now;
                existing.LicensedDevices.Add(new LicenseDevice { MacAddress = mac, HardwareFingerprint = fp, MachineName = machine, RegisteredAt = now });
            }
            else
            {
                var license = new LicenseInfo
                {
                    LicenseKey = licenseKey, LicenseType = licenseType, ActivationDate = now, ExpiryDate = expiry,
                    PrimaryMacAddress = mac, HardwareFingerprint = fp, MaxDevices = maxDevices,
                    RegisteredDeviceCount = 1, IsActivated = true, IsActive = true
                };
                license.LicensedDevices.Add(new LicenseDevice { MacAddress = mac, HardwareFingerprint = fp, MachineName = machine, RegisteredAt = now });
                await _licenseRepo.AddAsync(license);
            }

            await _context.SaveChangesAsync();
            var period = licenseType == LicenseType.Lifetime ? "lifetime" : licenseType == LicenseType.OneYear ? "1 year" : "3 months";
            return new LicenseResult { Success = true, Message = $"Activated! {period} license. {maxDevices} device(s) supported.", License = existing ?? await GetCurrentLicenseAsync() };
        }

        public async Task<LicenseResult> ValidateAsync()
        {
            var license = await GetCurrentLicenseAsync();
            if (license == null || !license.IsActivated)
                return new LicenseResult { Success = false, Message = "No active license found. Please activate the system." };
            if (license.LicenseType != LicenseType.Lifetime && license.ExpiryDate.HasValue && license.ExpiryDate.Value < DateTime.Now)
                return new LicenseResult { Success = false, Message = $"License expired on {license.ExpiryDate.Value:d}." };
            return new LicenseResult { Success = true, Message = "License is valid.", License = license };
        }

        public async Task<LicenseResult> RegisterCurrentDeviceAsync()
        {
            var license = await GetCurrentLicenseAsync();
            if (license == null || !license.IsActivated)
                return new LicenseResult { Success = false, Message = "No active license found." };
            if (license.RegisteredDeviceCount >= license.MaxDevices)
                return new LicenseResult { Success = false, Message = $"Maximum devices ({license.MaxDevices}) reached." };

            var mac = _hardwareInfo.GetMacAddress();
            var fp = _hardwareInfo.GetHardwareFingerprint();
            var machine = _hardwareInfo.GetMachineName();

            if (license.LicensedDevices.Any(d => d.MacAddress == mac || d.HardwareFingerprint == fp))
                return new LicenseResult { Success = true, Message = "Device already registered.", License = license };

            license.LicensedDevices.Add(new LicenseDevice { MacAddress = mac, HardwareFingerprint = fp, MachineName = machine, RegisteredAt = DateTime.Now });
            license.RegisteredDeviceCount = license.LicensedDevices.Count;
            license.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return new LicenseResult { Success = true, Message = $"Device registered. {license.MaxDevices - license.RegisteredDeviceCount} slot(s) remaining.", License = license };
        }

        public string GenerateLicenseKey(LicenseType type)
        {
            string prefix = type switch { LicenseType.ThreeMonths => "MEMDS-3M", LicenseType.OneYear => "MEMDS-1Y", _ => "MEMDS-LT" };
            var rnd = new Random();
            var segs = Enumerable.Range(0, 3).Select(_ => new string(Enumerable.Range(0, 4).Select(i => "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"[rnd.Next(36)]).ToArray())).ToArray();
            var baseKey = $"{prefix}-{string.Join("-", segs)}";
            return $"{baseKey}-{ComputeChecksum(baseKey)}";
        }

        public bool ValidateKeyFormat(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return false;
            var parts = key.Trim().Split('-');
            if (parts.Length != 6 || parts[0] != "MEMDS") return false;
            if (parts[1] != "3M" && parts[1] != "1Y" && parts[1] != "LT") return false;
            return parts.Skip(2).All(p => p.Length == 4 && p.All(char.IsLetterOrDigit));
        }

        public async Task<LicenseResult> RemoveDeviceAsync(int id)
        {
            var device = await _deviceRepo.GetByIdAsync(id);
            if (device == null) return new LicenseResult { Success = false, Message = "Device not found." };
            device.IsActive = false;
            device.UpdatedAt = DateTime.Now;
            _deviceRepo.Update(device);
            var license = await GetCurrentLicenseAsync();
            if (license != null) { license.RegisteredDeviceCount = license.LicensedDevices.Count(d => d.IsActive); license.UpdatedAt = DateTime.Now; }
            await _context.SaveChangesAsync();
            return new LicenseResult { Success = true, Message = $"Device removed. {license?.MaxDevices - license?.RegisteredDeviceCount ?? 0} slot(s) remaining.", License = license };
        }

        public async Task<int> GetRemainingSlotsAsync()
        {
            var license = await GetCurrentLicenseAsync();
            return license != null ? Math.Max(0, license.MaxDevices - license.RegisteredDeviceCount) : 0;
        }

        public async Task<bool> IsLifetimeLicenseAsync() => (await GetCurrentLicenseAsync())?.LicenseType == LicenseType.Lifetime;

        public async Task<string> GetLicenseStatusTextAsync()
        {
            var license = await GetCurrentLicenseAsync();
            if (license == null || !license.IsActivated) return "No License — Not Activated";
            if (license.LicenseType == LicenseType.Lifetime) return $"Lifetime — {license.RegisteredDeviceCount}/{license.MaxDevices} devices";
            if (license.ExpiryDate.HasValue)
            {
                var days = (license.ExpiryDate.Value - DateTime.Now).Days;
                return days <= 0 ? $"EXPIRED on {license.ExpiryDate.Value:d}" : days <= 30 ? $"{days} day(s) remaining — EXPIRING SOON" : $"{days} day(s) remaining";
            }
            return "License Active";
        }

        public async Task<LicenseResult> UpdateHospitalInfoAsync(string hospitalName, string administratorName, string licenseNumber)
        {
            var license = await GetCurrentLicenseAsync();
            if (license == null)
            {
                // Create a new license record with just the hospital info
                license = new LicenseInfo
                {
                    HospitalName = hospitalName.Trim(),
                    AdministratorName = administratorName.Trim(),
                    LicenseNumber = licenseNumber.Trim(),
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };
                await _context.Licenses.AddAsync(license);
            }
            else
            {
                license.HospitalName = hospitalName.Trim();
                license.AdministratorName = administratorName.Trim();
                license.LicenseNumber = licenseNumber.Trim();
                license.UpdatedAt = DateTime.Now;
            }
            await _context.SaveChangesAsync();
            return new LicenseResult { Success = true, Message = "Hospital information saved successfully.", License = license };
        }

        public async Task<LicenseResult> GenerateLicenseFileAsync(string outputPath, LicenseType licenseType, string hospitalName, string administratorName, string licenseNumber)
        {
            try
            {
                // Generate the license key
                var licenseKey = GenerateLicenseKey(licenseType);

                // Calculate max devices based on type
                int maxDevices = licenseType == LicenseType.Lifetime ? 10 : 1;

                // Calculate expiry
                var now = DateTime.Now;
                DateTime? expiry = licenseType switch
                {
                    LicenseType.ThreeMonths => now.AddMonths(3),
                    LicenseType.OneYear => now.AddYears(1),
                    _ => null
                };

                // Get hardware info for binding
                var mac = _hardwareInfo.GetMacAddress();
                var fp = _hardwareInfo.GetHardwareFingerprint();
                var machine = _hardwareInfo.GetMachineName();

                // Create license file content as JSON
                var licenseData = new
                {
                    ProductName = "MomenMedmSys",
                    Company = "Momen Systems Co.",
                    LicenseKey = licenseKey,
                    LicenseType = licenseType.ToString(),
                    HospitalName = hospitalName.Trim(),
                    AdministratorName = administratorName.Trim(),
                    LicenseNumber = licenseNumber.Trim(),
                    ActivationDate = now.ToString("yyyy-MM-dd HH:mm:ss"),
                    ExpiryDate = expiry?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Lifetime",
                    MaxDevices = maxDevices,
                    HardwareFingerprint = fp,
                    MacAddress = mac,
                    MachineName = machine,
                    GeneratedOn = now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Checksum = ComputeChecksum(licenseKey.Split('-').Take(5).Aggregate((a, b) => $"{a}-{b}"))
                };

                var jsonContent = System.Text.Json.JsonSerializer.Serialize(licenseData, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });

                // Ensure directory exists
                var directory = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Write license file
                await File.WriteAllTextAsync(outputPath, jsonContent);

                return new LicenseResult
                {
                    Success = true,
                    Message = $"License file generated: {Path.GetFileName(outputPath)}",
                    License = new LicenseInfo
                    {
                        LicenseKey = licenseKey,
                        LicenseType = licenseType,
                        HospitalName = hospitalName.Trim(),
                        AdministratorName = administratorName.Trim(),
                        LicenseNumber = licenseNumber.Trim(),
                        MaxDevices = maxDevices,
                        ExpiryDate = expiry,
                        HardwareFingerprint = fp,
                        PrimaryMacAddress = mac,
                        IsActivated = false,
                        IsActive = true,
                        CreatedAt = now
                    }
                };
            }
            catch (Exception ex)
            {
                return new LicenseResult { Success = false, Message = $"Failed to generate license file: {ex.Message}" };
            }
        }

        public async Task<LicenseResult> UpdateSystemExecutableAsync(string outputPath, LicenseType licenseType, string hospitalName, string administratorName, string licenseNumber)
        {
            try
            {
                // First, generate the license file
                var licenseFileName = Path.GetFileNameWithoutExtension(outputPath) + ".license";
                var licenseFilePath = Path.Combine(Path.GetDirectoryName(outputPath) ?? ".", licenseFileName);

                var licenseResult = await GenerateLicenseFileAsync(licenseFilePath, licenseType, hospitalName, administratorName, licenseNumber);

                if (!licenseResult.Success)
                {
                    return licenseResult;
                }

                // Now update/save the database record
                var license = await GetCurrentLicenseAsync();
                if (license == null)
                {
                    var now = DateTime.Now;
                    DateTime? expiry = licenseType switch
                    {
                        LicenseType.ThreeMonths => now.AddMonths(3),
                        LicenseType.OneYear => now.AddYears(1),
                        _ => null
                    };

                    license = new LicenseInfo
                    {
                        LicenseKey = licenseResult.License!.LicenseKey,
                        LicenseType = licenseType,
                        HospitalName = hospitalName.Trim(),
                        AdministratorName = administratorName.Trim(),
                        LicenseNumber = licenseNumber.Trim(),
                        ActivationDate = now,
                        ExpiryDate = expiry,
                        PrimaryMacAddress = _hardwareInfo.GetMacAddress(),
                        HardwareFingerprint = _hardwareInfo.GetHardwareFingerprint(),
                        MaxDevices = licenseType == LicenseType.Lifetime ? 10 : 1,
                        RegisteredDeviceCount = 1,
                        IsActivated = true,
                        IsActive = true,
                        CreatedAt = now
                    };

                    license.LicensedDevices.Add(new LicenseDevice
                    {
                        MacAddress = _hardwareInfo.GetMacAddress(),
                        HardwareFingerprint = _hardwareInfo.GetHardwareFingerprint(),
                        MachineName = _hardwareInfo.GetMachineName(),
                        RegisteredAt = now
                    });

                    await _context.Licenses.AddAsync(license);
                }
                else
                {
                    license.LicenseKey = licenseResult.License!.LicenseKey;
                    license.LicenseType = licenseType;
                    license.HospitalName = hospitalName.Trim();
                    license.AdministratorName = administratorName.Trim();
                    license.LicenseNumber = licenseNumber.Trim();
                    license.UpdatedAt = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                return new LicenseResult
                {
                    Success = true,
                    Message = $"System package updated! License file: {Path.GetFileName(licenseFilePath)}",
                    License = license
                };
            }
            catch (Exception ex)
            {
                return new LicenseResult { Success = false, Message = $"Failed to update system executable: {ex.Message}" };
            }
        }

        private bool VerifyKeyChecksum(string key)
        {
            var parts = key.Trim().Split('-');
            return parts[5].Equals(ComputeChecksum(string.Join("-", parts.Take(5))), StringComparison.OrdinalIgnoreCase);
        }

        private static string ComputeChecksum(string keyBase)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(keyBase + "MomenSystems2026"));
            var hex = BitConverter.ToString(hash).Replace("-", "");
            var charset = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var sb = new StringBuilder();
            for (int i = 0; i < 4; i++) sb.Append(charset[Convert.ToInt32(hex.Substring(i * 2, 2), 16) % charset.Length]);
            return sb.ToString().ToUpper();
        }
    }
}
