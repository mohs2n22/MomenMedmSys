using System;
using System.Threading.Tasks;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MomenMedmSys.Data
{
    public static class DatabaseSeeder
    {
        public static async Task<bool> SeedAsync(IServiceProvider serviceProvider)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<MedMsysDbContext>();
                await ctx.Database.MigrateAsync();

                bool hasDevices = await ctx.MedicalDevices.AnyAsync();
                bool hasAdmin = await ctx.StaffMembers.AnyAsync(s => s.Username == "admin");

                if (hasDevices && hasAdmin)
                    return false;

                // Seed in order with proper relationships
                await SeedBasicDataAsync(ctx, hasDevices, hasAdmin);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Seeder] Failed: {ex.Message}");
                return false;
            }
        }

        private static async Task SeedBasicDataAsync(MedMsysDbContext ctx, bool skipDevices, bool skipAdmin)
        {
            var now = DateTime.Now;

            // 1. Departments (only if no devices exist)
            if (!skipDevices)
            {
            ctx.Departments.AddRange(new[]
            {
                new Department { Id = 1, DepartmentCode = "RAD", Name = "Radiology", Manager = "Dr. Ahmed Hassan", Building = "Main", Floor = "1", Budget = 500000, CreatedAt = now, IsActive = true },
                new Department { Id = 2, DepartmentCode = "ICU", Name = "Intensive Care", Manager = "Dr. James Wilson", Building = "Main", Floor = "2", Budget = 800000, CreatedAt = now, IsActive = true },
                new Department { Id = 3, DepartmentCode = "LAB", Name = "Laboratory", Manager = "Mohamed Ali", Building = "West", Floor = "1", Budget = 350000, CreatedAt = now, IsActive = true },
                new Department { Id = 4, DepartmentCode = "ER", Name = "Emergency", Manager = "Dr. Emily Chen", Building = "Main", Floor = "G", Budget = 600000, CreatedAt = now, IsActive = true },
                new Department { Id = 5, DepartmentCode = "OR", Name = "Operating Room", Manager = "Dr. Khalid Omar", Building = "Main", Floor = "3", Budget = 700000, CreatedAt = now, IsActive = true },
            });
            await ctx.SaveChangesAsync();
            }

            // 2. Staff Members (Admin user)
            if (!skipAdmin)
            {
                var adminPasswordHash = Convert.ToBase64String(
                    System.Security.Cryptography.SHA256.HashData(
                        System.Text.Encoding.UTF8.GetBytes("Admin@123")));

                ctx.StaffMembers.Add(new StaffMember
                {
                    Id = 1,
                    EmployeeId = "EMP-ADMIN",
                    FirstName = "System",
                    LastName = "Administrator",
                    Email = "admin@medmsys.local",
                    Phone = "+1-555-0000",
                    Role = StaffRole.Administrator,
                    SubRole = "System Administrator",
                    Department = "IT Department",
                    JobTitle = "System Administrator",
                    Username = "admin",
                    PasswordHash = adminPasswordHash,
                    IsActiveAccount = true,
                    IsLocked = false,
                    FailedLoginAttempts = 0,
                    HireDate = now,
                    // Full admin permissions
                    CanManageDevices = true,
                    CanManageMaintenance = true,
                    CanManageCalibration = true,
                    CanManageSpareParts = true,
                    CanViewReports = true,
                    CanManageNetworkDevices = true,
                    CanManageStaff = true,
                    CanAccessAdminPanel = true,
                    CreatedAt = now,
                    IsActive = true
                });
                await ctx.SaveChangesAsync();
            }

            // 3. Suppliers (only if no devices exist)
            if (!skipDevices)
            {
            ctx.Suppliers.AddRange(new[]
            {
                new Supplier { Id = 1, SupplierCode = "SUP-001", CompanyName = "MedTech Industries", ContactPerson = "Robert Williams", Email = "r@medtech.com", Phone = "+1-555-1001", City = "Boston", Country = "USA", Rating = 5, IsApproved = true, LeadTimeDays = 30, CreatedAt = now, IsActive = true },
                new Supplier { Id = 2, SupplierCode = "SUP-002", CompanyName = "VitalCare Systems", ContactPerson = "Jennifer Davis", Email = "j@vitalcare.com", Phone = "+1-555-2001", City = "Minneapolis", Country = "USA", Rating = 4, IsApproved = true, LeadTimeDays = 14, CreatedAt = now, IsActive = true },
                new Supplier { Id = 3, SupplierCode = "SUP-003", CompanyName = "RespiraTech Inc.", ContactPerson = "Sarah Mitchell", Email = "s@respiratech.com", Phone = "+1-555-4001", City = "Denver", Country = "USA", Rating = 5, IsApproved = true, LeadTimeDays = 28, CreatedAt = now, IsActive = true },
            });
            await ctx.SaveChangesAsync();

            // 3. Devices (with FK references)
            ctx.MedicalDevices.AddRange(new[]
            {
                new MedicalDevice { Id = 1, DeviceCode = "DEV-001", DeviceName = "X-Ray Machine XR-200", Manufacturer = "MedTech Industries", Model = "XR-200", SerialNumber = "XR200-2023-001", Category = "Imaging", PurchaseDate = new DateTime(2023,1,15), PurchasePrice = 85000m, SupplierId = 1, SupplierName = "MedTech", WarrantyExpiryDate = new DateTime(2026,1,15), DepartmentId = 1, Department = "Radiology", Building = "Main", Floor = "1", Room = "101", AssignedTo = "Dr. Ahmed", TechnicalSpecs = "120kV, 500mA", PowerRequirements = "220V AC", RequiresCalibration = true, RequiresPreventiveMaintenance = true, Status = DeviceStatus.Active, RiskClassification = RiskClass.High, CreatedAt = now, IsActive = true },
                new MedicalDevice { Id = 2, DeviceCode = "DEV-002", DeviceName = "Patient Monitor PM-500", Manufacturer = "VitalCare", Model = "PM-500", SerialNumber = "PM500-2023-045", Category = "Monitoring", PurchaseDate = new DateTime(2023,3,22), PurchasePrice = 12500m, SupplierId = 2, SupplierName = "VitalCare", WarrantyExpiryDate = new DateTime(2026,3,22), DepartmentId = 2, Department = "ICU", Building = "Main", Floor = "2", Room = "205", AssignedTo = "Nurse Sarah", TechnicalSpecs = "ECG 12-lead, SpO2", PowerRequirements = "100-240V AC", RequiresCalibration = true, RequiresPreventiveMaintenance = true, Status = DeviceStatus.Active, RiskClassification = RiskClass.Critical, CreatedAt = now, IsActive = true },
                new MedicalDevice { Id = 3, DeviceCode = "DEV-003", DeviceName = "Infusion Pump IP-100", Manufacturer = "MediFlow", Model = "IP-100", SerialNumber = "IP100-2022-112", Category = "Therapeutic", PurchaseDate = new DateTime(2022,8,10), PurchasePrice = 3200m, SupplierName = "MediFlow", WarrantyExpiryDate = new DateTime(2025,8,10), DepartmentId = 2, Department = "ICU", Building = "East", Floor = "3", Room = "312", AssignedTo = "Dr. Fatima", TechnicalSpecs = "0.1-999 ml/hr", PowerRequirements = "100-240V AC", RequiresCalibration = true, RequiresPreventiveMaintenance = true, Status = DeviceStatus.UnderMaintenance, RiskClassification = RiskClass.High, CreatedAt = now, IsActive = true },
                new MedicalDevice { Id = 4, DeviceCode = "DEV-004", DeviceName = "Ventilator VH-300", Manufacturer = "RespiraTech", Model = "VH-300", SerialNumber = "VH300-2023-078", Category = "Therapeutic", PurchaseDate = new DateTime(2023,6,1), PurchasePrice = 28000m, SupplierId = 3, SupplierName = "RespiraTech", WarrantyExpiryDate = new DateTime(2026,6,1), DepartmentId = 2, Department = "ICU", Building = "Main", Floor = "2", Room = "208", AssignedTo = "Dr. James", TechnicalSpecs = "Volume/Pressure control", PowerRequirements = "100-240V AC", RequiresCalibration = true, RequiresPreventiveMaintenance = true, Status = DeviceStatus.Active, RiskClassification = RiskClass.Critical, CreatedAt = now, IsActive = true },
                new MedicalDevice { Id = 5, DeviceCode = "DEV-005", DeviceName = "Blood Analyzer BA-450", Manufacturer = "LabTech", Model = "BA-450", SerialNumber = "BA450-2023-034", Category = "Laboratory", PurchaseDate = new DateTime(2023,2,28), PurchasePrice = 45000m, SupplierName = "LabTech", WarrantyExpiryDate = new DateTime(2026,2,28), DepartmentId = 3, Department = "Laboratory", Building = "West", Floor = "1", Room = "103", AssignedTo = "Mohamed Ali", TechnicalSpecs = "CBC, 60 samples/hr", PowerRequirements = "220V AC", RequiresCalibration = true, RequiresPreventiveMaintenance = true, Status = DeviceStatus.Active, RiskClassification = RiskClass.High, CreatedAt = now, IsActive = true },
                new MedicalDevice { Id = 6, DeviceCode = "DEV-006", DeviceName = "Defibrillator DF-200", Manufacturer = "CardioLife", Model = "DF-200", SerialNumber = "DF200-2023-056", Category = "Therapeutic", PurchaseDate = new DateTime(2023,4,15), PurchasePrice = 15000m, SupplierName = "CardioLife", WarrantyExpiryDate = new DateTime(2026,4,15), DepartmentId = 4, Department = "Emergency", Building = "Main", Floor = "G", Room = "ER-002", AssignedTo = "Dr. Emily", TechnicalSpecs = "Biphasic 1-360J", PowerRequirements = "Battery", RequiresCalibration = true, RequiresPreventiveMaintenance = true, Status = DeviceStatus.Active, RiskClassification = RiskClass.Critical, CreatedAt = now, IsActive = true },
                new MedicalDevice { Id = 7, DeviceCode = "DEV-007", DeviceName = "Ultrasound US-600", Manufacturer = "SonoVision", Model = "US-600", SerialNumber = "US600-2023-089", Category = "Imaging", PurchaseDate = new DateTime(2023,7,20), PurchasePrice = 55000m, SupplierName = "SonoVision", WarrantyExpiryDate = new DateTime(2026,7,20), DepartmentId = 5, Department = "Operating Room", Building = "Main", Floor = "2", Room = "215", AssignedTo = "Dr. Layla", TechnicalSpecs = "4D Doppler", PowerRequirements = "100-240V AC", RequiresCalibration = false, RequiresPreventiveMaintenance = true, Status = DeviceStatus.Active, RiskClassification = RiskClass.Medium, CreatedAt = now, IsActive = true },
                new MedicalDevice { Id = 8, DeviceCode = "DEV-008", DeviceName = "CT Scanner CT-800", Manufacturer = "MedTech Industries", Model = "CT-800", SerialNumber = "CT800-2022-023", Category = "Imaging", PurchaseDate = new DateTime(2022,11,5), PurchasePrice = 250000m, SupplierId = 1, SupplierName = "MedTech", WarrantyExpiryDate = new DateTime(2025,11,5), DepartmentId = 1, Department = "Radiology", Building = "Main", Floor = "G", Room = "CT-Suite", AssignedTo = "Dr. Ahmed", TechnicalSpecs = "64-slice, 0.5mm", PowerRequirements = "380V 3-phase", RequiresCalibration = true, RequiresPreventiveMaintenance = true, Status = DeviceStatus.UnderMaintenance, RiskClassification = RiskClass.High, CreatedAt = now, IsActive = true },
                new MedicalDevice { Id = 9, DeviceCode = "DEV-009", DeviceName = "Anesthesia Machine AM-150", Manufacturer = "RespiraTech", Model = "AM-150", SerialNumber = "AM150-2023-067", Category = "Therapeutic", PurchaseDate = new DateTime(2023,5,12), PurchasePrice = 32000m, SupplierId = 3, SupplierName = "RespiraTech", WarrantyExpiryDate = new DateTime(2026,5,12), DepartmentId = 5, Department = "Operating Room", Building = "Main", Floor = "3", Room = "OR-301", AssignedTo = "Dr. Khalid", TechnicalSpecs = "Ventilation, vaporizers", PowerRequirements = "100-240V AC", RequiresCalibration = true, RequiresPreventiveMaintenance = true, Status = DeviceStatus.Active, RiskClassification = RiskClass.Critical, CreatedAt = now, IsActive = true },
                new MedicalDevice { Id = 10, DeviceCode = "DEV-010", DeviceName = "ECG Machine EC-90", Manufacturer = "VitalCare", Model = "EC-90", SerialNumber = "EC90-2023-091", Category = "Diagnostic", PurchaseDate = new DateTime(2023,8,5), PurchasePrice = 4500m, SupplierId = 2, SupplierName = "VitalCare", WarrantyExpiryDate = new DateTime(2026,8,5), DepartmentId = 4, Department = "Emergency", Building = "Main", Floor = "2", Room = "220", AssignedTo = "Dr. Emily", TechnicalSpecs = "12-lead", PowerRequirements = "100-240V AC", RequiresCalibration = true, RequiresPreventiveMaintenance = true, Status = DeviceStatus.Active, RiskClassification = RiskClass.Medium, CreatedAt = now, IsActive = true },
            });
            await ctx.SaveChangesAsync();

            // 4. Maintenance
            ctx.MaintenanceRecords.AddRange(new[]
            {
                new MaintenanceRecord { Id = 1, DeviceId = 1, Type = MaintenanceType.Preventive, Title = "Quarterly PM - X-Ray", ScheduledDate = now.AddDays(7), NextDueDate = now.AddDays(97), Recurrence = RecurrenceFrequency.Quarterly, RecurrenceInterval = 3, PerformedBy = "Biomedical Team", Status = MaintenanceStatus.Scheduled, LaborCost = 250m, CreatedAt = now, IsActive = true },
                new MaintenanceRecord { Id = 2, DeviceId = 3, Type = MaintenanceType.Corrective, Title = "Emergency Repair - Pump Motor", ScheduledDate = now.AddDays(-3), CompletedDate = now.AddDays(-2), PerformedBy = "John Smith", Status = MaintenanceStatus.Completed, Findings = "Motor bearing worn", ActionsTaken = "Replaced motor", LaborCost = 150m, PartsCost = 320m, CreatedAt = now, IsActive = true },
                new MaintenanceRecord { Id = 3, DeviceId = 4, Type = MaintenanceType.Preventive, Title = "Monthly PM - Ventilator", ScheduledDate = now.AddDays(5), NextDueDate = now.AddDays(35), Recurrence = RecurrenceFrequency.Monthly, RecurrenceInterval = 1, PerformedBy = "Biomedical Team", Status = MaintenanceStatus.Scheduled, LaborCost = 120m, CreatedAt = now, IsActive = true },
            });
            await ctx.SaveChangesAsync();

            // 5. Calibration
            ctx.CalibrationRecords.AddRange(new[]
            {
                new CalibrationRecord { Id = 1, DeviceId = 1, CalibrationType = "Full Calibration", StandardUsed = "NIST-traceable", CalibrationDate = now.AddDays(-30), NextDueDate = now.AddDays(150), PerformedBy = "MetroLab", IsExternalLab = true, LaboratoryName = "MetroLab", Result = CalibrationResult.Pass, AsFoundData = "kVp: 118.5", AsLeftData = "kVp: 120.1", CertificateNumber = "CERT-001", CreatedAt = now, IsActive = true },
                new CalibrationRecord { Id = 2, DeviceId = 2, CalibrationType = "Verification", StandardUsed = "Simulator", CalibrationDate = now.AddDays(-15), NextDueDate = now.AddDays(165), PerformedBy = "Internal Team", Result = CalibrationResult.Pass, AsFoundData = "Within spec", AsLeftData = "Within spec", CertificateNumber = "INT-001", CreatedAt = now, IsActive = true },
            });
            await ctx.SaveChangesAsync();

            // 6. Spare Parts
            ctx.SpareParts.AddRange(new[]
            {
                new SparePart { Id = 1, PartNumber = "SP-001", PartName = "X-Ray Tube Assembly", DeviceId = 1, Category = "Imaging", SupplierId = 1, SupplierName = "MedTech", CurrentStock = 2, MinimumStock = 1, MaximumStock = 3, ReorderPoint = 1, UnitCost = 12500m, StorageLocation = "Warehouse A", IsCritical = true, CreatedAt = now, IsActive = true },
                new SparePart { Id = 2, PartNumber = "SP-002", PartName = "Monitor Cable Set", DeviceId = 2, Category = "Cables", SupplierId = 2, SupplierName = "VitalCare", CurrentStock = 8, MinimumStock = 10, MaximumStock = 20, ReorderPoint = 5, UnitCost = 85m, StorageLocation = "Warehouse B", IsCritical = true, CreatedAt = now, IsActive = true },
                new SparePart { Id = 3, PartNumber = "SP-003", PartName = "Infusion Pump Motor", DeviceId = 3, Category = "Motors", SupplierName = "MediFlow", CurrentStock = 3, MinimumStock = 2, MaximumStock = 5, ReorderPoint = 2, UnitCost = 320m, StorageLocation = "Warehouse B", IsCritical = true, CreatedAt = now, IsActive = true },
                new SparePart { Id = 4, PartNumber = "SP-004", PartName = "Ventilator HEPA Filter", DeviceId = 4, Category = "Filters", SupplierId = 3, SupplierName = "RespiraTech", CurrentStock = 15, MinimumStock = 10, MaximumStock = 30, ReorderPoint = 12, UnitCost = 45m, StorageLocation = "Warehouse A", IsCritical = true, CreatedAt = now, IsActive = true },
                new SparePart { Id = 5, PartNumber = "SP-005", PartName = "Defibrillator Battery", DeviceId = 6, Category = "Batteries", SupplierName = "CardioLife", CurrentStock = 4, MinimumStock = 3, MaximumStock = 6, ReorderPoint = 3, UnitCost = 450m, StorageLocation = "Warehouse B", IsCritical = true, CreatedAt = now, IsActive = true },
            });
            await ctx.SaveChangesAsync();

            // 7. Service Contracts
            ctx.ServiceContracts.AddRange(new[]
            {
                new ServiceContract { Id = 1, ContractNumber = "SC-2024-001", ContractName = "Imaging Service", SupplierId = 1, Provider = "MedTech", ContactPerson = "Robert", StartDate = new DateTime(2024,1,1), EndDate = new DateTime(2025,12,31), AutoRenew = true, CoverageDescription = "Full imaging coverage", ContractValue = 45000m, ResponseTimeHours = 4, ResolutionTimeHours = 24, TotalCalls = 12, CompletedCalls = 10, SatisfactionScore = 4.5m, Status = ContractStatus.Active, CreatedAt = now, IsActive = true },
                new ServiceContract { Id = 2, ContractNumber = "SC-2024-002", ContractName = "ICU Maintenance", SupplierId = 3, Provider = "RespiraTech", ContactPerson = "Sarah", StartDate = new DateTime(2024,3,1), EndDate = new DateTime(2025,2,28), AutoRenew = false, CoverageDescription = "ICU devices", ContractValue = 28000m, ResponseTimeHours = 2, ResolutionTimeHours = 8, TotalCalls = 18, CompletedCalls = 16, SatisfactionScore = 4.2m, Status = ContractStatus.Active, CreatedAt = now, IsActive = true },
            });
            await ctx.SaveChangesAsync();

            System.Diagnostics.Debug.WriteLine("[Seeder] Seeded: admin user, 5 depts, 3 suppliers, 10 devices, 3 maintenance, 2 calibration, 5 parts, 2 contracts");
            }
        }
    }
}
