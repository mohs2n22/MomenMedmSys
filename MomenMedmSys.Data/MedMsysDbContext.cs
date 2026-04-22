using MomenMedmSys.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MomenMedmSys.Data
{
    public class MedMsysDbContext : DbContext
    {
        public MedMsysDbContext(DbContextOptions<MedMsysDbContext> options) : base(options)
        {
        }

        // Core Asset Management
        public DbSet<MedicalDevice> MedicalDevices { get; set; }

        // Maintenance Management
        public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }

        // Calibration & Verification
        public DbSet<CalibrationRecord> CalibrationRecords { get; set; }

        // Risk & Safety Management
        public DbSet<RiskIncident> RiskIncidents { get; set; }

        // Spare Parts & Inventory
        public DbSet<SparePart> SpareParts { get; set; }
        public DbSet<SparePartUsage> SparePartUsages { get; set; }

        // Service Contracts
        public DbSet<ServiceContract> ServiceContracts { get; set; }

        // Organizational
        public DbSet<Department> Departments { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<StaffMember> StaffMembers { get; set; }
        public DbSet<TrainingRecord> TrainingRecords { get; set; }

        // Work Orders & Procurement
        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<ProcurementRequest> ProcurementRequests { get; set; }
        public DbSet<TechnicalEvaluation> TechnicalEvaluations { get; set; }

        // Documents & Safety
        public DbSet<DeviceDocument> DeviceDocuments { get; set; }
        public DbSet<ElectricalSafetyTest> ElectricalSafetyTests { get; set; }

        // Network Devices
        public DbSet<NetworkDevice> NetworkDevices { get; set; }
        public DbSet<DeviceActionLog> DeviceActionLogs { get; set; }

        // Staff Assignments
        public DbSet<AssignedDevice> AssignedDevices { get; set; }

        // Authentication & User Management
        public DbSet<User> Users { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }

        // Audit Logging
        public DbSet<AuditLog> AuditLogs { get; set; }

        // Notifications & Alerts
        public DbSet<Notification> Notifications { get; set; }

        // Licensing
        public DbSet<LicenseInfo> Licenses { get; set; }
        public DbSet<LicenseDevice> LicensedDevices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // MedicalDevice configuration
            modelBuilder.Entity<MedicalDevice>(entity =>
            {
                entity.HasIndex(d => d.DeviceCode).IsUnique();
                entity.HasIndex(d => d.SerialNumber);
                entity.Property(d => d.DeviceName).IsRequired().HasMaxLength(200);
                entity.Property(d => d.Manufacturer).HasMaxLength(200);
                entity.Property(d => d.Model).HasMaxLength(100);
                entity.Property(d => d.SerialNumber).HasMaxLength(100);
                entity.Property(d => d.PurchasePrice).HasColumnType("REAL");
            });

            // MaintenanceRecord configuration
            modelBuilder.Entity<MaintenanceRecord>(entity =>
            {
                entity.HasIndex(m => m.DeviceId);
                entity.HasIndex(m => m.ScheduledDate);
                entity.Property(m => m.LaborCost).HasColumnType("REAL");
                entity.Property(m => m.PartsCost).HasColumnType("REAL");

                entity.HasOne(m => m.Device)
                      .WithMany(d => d.MaintenanceRecords)
                      .HasForeignKey(m => m.DeviceId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // CalibrationRecord configuration
            modelBuilder.Entity<CalibrationRecord>(entity =>
            {
                entity.HasIndex(c => c.DeviceId);
                entity.HasIndex(c => c.CalibrationDate);
                entity.HasIndex(c => c.NextDueDate);

                entity.HasOne(c => c.Device)
                      .WithMany(d => d.CalibrationRecords)
                      .HasForeignKey(c => c.DeviceId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // RiskIncident configuration
            modelBuilder.Entity<RiskIncident>(entity =>
            {
                entity.HasIndex(i => i.IncidentCode).IsUnique();
                entity.HasIndex(i => i.DeviceId);
                entity.HasIndex(i => i.IncidentDate);

                entity.HasOne(i => i.Device)
                      .WithMany(d => d.RiskIncidents)
                      .HasForeignKey(i => i.DeviceId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // SparePart configuration
            modelBuilder.Entity<SparePart>(entity =>
            {
                entity.HasIndex(p => p.PartNumber).IsUnique();
                entity.Property(p => p.UnitCost).HasColumnType("REAL");
            });

            // SparePartUsage configuration
            modelBuilder.Entity<SparePartUsage>(entity =>
            {
                entity.HasOne(su => su.SparePart)
                      .WithMany(sp => sp.Usages)
                      .HasForeignKey(su => su.SparePartId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(su => su.MaintenanceRecord)
                      .WithMany(mr => mr.SparePartUsages)
                      .HasForeignKey(su => su.MaintenanceRecordId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ServiceContract configuration
            modelBuilder.Entity<ServiceContract>(entity =>
            {
                entity.HasIndex(c => c.ContractNumber).IsUnique();
                entity.Property(c => c.ContractValue).HasColumnType("REAL");
            });

            // Department configuration
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasIndex(d => d.DepartmentCode).IsUnique();
                entity.Property(d => d.Name).IsRequired().HasMaxLength(200);
            });

            // Supplier configuration
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasIndex(s => s.SupplierCode).IsUnique();
                entity.Property(s => s.CompanyName).IsRequired().HasMaxLength(200);
                entity.Property(s => s.Email).HasMaxLength(200);
            });

            // StaffMember configuration
            modelBuilder.Entity<StaffMember>(entity =>
            {
                entity.HasIndex(s => s.EmployeeId).IsUnique();
                entity.Property(s => s.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(s => s.LastName).IsRequired().HasMaxLength(100);
            });

            // TrainingRecord configuration
            modelBuilder.Entity<TrainingRecord>(entity =>
            {
                entity.HasIndex(t => new { t.StaffMemberId, t.DeviceId, t.TrainingDate });
                entity.Property(t => t.TrainingTitle).IsRequired().HasMaxLength(200);
            });

            // WorkOrder configuration
            modelBuilder.Entity<WorkOrder>(entity =>
            {
                entity.HasIndex(w => w.WorkOrderNumber).IsUnique();
                entity.HasIndex(w => w.DeviceId);
                entity.HasIndex(w => w.Status);
                entity.HasIndex(w => w.ReportDate);

                entity.HasOne(w => w.Device)
                      .WithMany(d => d.WorkOrders)
                      .HasForeignKey(w => w.DeviceId)
                      .OnDelete(DeleteBehavior.Restrict);

                // One-to-one with MaintenanceRecord (optional on both sides)
                entity.HasOne(w => w.MaintenanceRecord)
                      .WithOne(m => m.WorkOrder)
                      .HasForeignKey<WorkOrder>(w => w.MaintenanceRecordId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ProcurementRequest configuration
            modelBuilder.Entity<ProcurementRequest>(entity =>
            {
                entity.HasIndex(p => p.RequestNumber).IsUnique();
                entity.HasIndex(p => p.Status);
                entity.Property(p => p.BudgetEstimate).HasColumnType("REAL");
                entity.Property(p => p.BudgetApproved).HasColumnType("REAL");

                entity.HasMany(p => p.TechnicalEvaluations)
                      .WithOne(te => te.ProcurementRequest)
                      .HasForeignKey(te => te.ProcurementRequestId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // TechnicalEvaluation configuration
            modelBuilder.Entity<TechnicalEvaluation>(entity =>
            {
                entity.HasIndex(te => new { te.ProcurementRequestId, te.SupplierId });
                entity.Property(te => te.QuotedPrice).HasColumnType("REAL");
                entity.Property(te => te.TotalCostOfOwnership).HasColumnType("REAL");
            });

            // DeviceDocument configuration
            modelBuilder.Entity<DeviceDocument>(entity =>
            {
                entity.HasIndex(d => new { d.DeviceId, d.DocumentType });
                entity.Property(d => d.FileName).IsRequired().HasMaxLength(255);
                entity.Property(d => d.FileSize).HasColumnType("INTEGER");
            });

            // ElectricalSafetyTest configuration
            modelBuilder.Entity<ElectricalSafetyTest>(entity =>
            {
                entity.HasIndex(e => e.DeviceId);
                entity.HasIndex(e => e.TestDate);
                entity.HasIndex(e => e.NextDueDate);

                entity.HasOne(e => e.Device)
                      .WithMany(d => d.SafetyTests)
                      .HasForeignKey(e => e.DeviceId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // NetworkDevice configuration
            modelBuilder.Entity<NetworkDevice>(entity =>
            {
                entity.HasIndex(n => n.IpAddress);
                entity.HasIndex(n => n.MacAddress);
                entity.HasIndex(n => n.Hostname);
                entity.HasIndex(n => n.ConnectionStatus);
                entity.HasIndex(n => n.DeviceName);
                entity.Property(n => n.IpAddress).HasMaxLength(45);
                entity.Property(n => n.MacAddress).HasMaxLength(17);
                entity.Property(n => n.Hostname).HasMaxLength(253);
            });

            // DeviceActionLog configuration
            modelBuilder.Entity<DeviceActionLog>(entity =>
            {
                entity.HasIndex(a => a.NetworkDeviceId);
                entity.HasIndex(a => a.ActionType);
                entity.HasIndex(a => a.Result);
                entity.HasIndex(a => a.CreatedAt);

                entity.HasOne(a => a.NetworkDevice)
                      .WithMany(d => d.ActionLogs)
                      .HasForeignKey(a => a.NetworkDeviceId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // AssignedDevice configuration
            modelBuilder.Entity<AssignedDevice>(entity =>
            {
                entity.HasIndex(ad => new { ad.StaffMemberId, ad.DeviceId });
                entity.HasOne(ad => ad.StaffMember).WithMany(s => s.AssignedDevices).HasForeignKey(ad => ad.StaffMemberId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(ad => ad.Device).WithMany(d => d.AssignedDevices).HasForeignKey(ad => ad.DeviceId).OnDelete(DeleteBehavior.Restrict);
            });

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
                entity.Property(u => u.FullName).IsRequired().HasMaxLength(200);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Email).HasMaxLength(200);
            });

            // UserSession configuration
            modelBuilder.Entity<UserSession>(entity =>
            {
                entity.HasIndex(s => s.UserId);
                entity.HasIndex(s => s.IsActive);
                entity.Property(s => s.IpAddress).HasMaxLength(45);
                entity.HasOne(s => s.User).WithMany(u => u.Sessions).HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
            });

            // AuditLog configuration
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasIndex(a => a.EntityType);
                entity.HasIndex(a => a.EntityId);
                entity.HasIndex(a => a.Action);
                entity.HasIndex(a => a.UserId);
                entity.HasIndex(a => a.Timestamp);
                entity.HasIndex(a => a.UserName);
                entity.Property(a => a.EntityType).IsRequired().HasMaxLength(100);
                entity.Property(a => a.Action).IsRequired().HasMaxLength(20);
                entity.Property(a => a.UserName).IsRequired().HasMaxLength(200);
                entity.Property(a => a.IpAddress).HasMaxLength(45);
                entity.Property(a => a.AffectedRecords).HasMaxLength(500);
            });

            // Notification configuration
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasIndex(n => n.UserId);
                entity.HasIndex(n => n.IsRead);
                entity.HasIndex(n => n.Type);
                entity.HasIndex(n => n.Priority);
                entity.HasIndex(n => n.DueDate);
                entity.Property(n => n.Title).IsRequired().HasMaxLength(200);
                entity.Property(n => n.Message).IsRequired().HasMaxLength(1000);
                entity.Property(n => n.EntityType).HasMaxLength(100);
                entity.Property(n => n.ActionUrl).HasMaxLength(500);
                entity.HasOne(n => n.User).WithMany().HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.SetNull);
            });

            // LicenseInfo configuration
            modelBuilder.Entity<LicenseInfo>(entity =>
            {
                entity.HasIndex(l => l.LicenseKey).IsUnique();
                entity.Property(l => l.LicenseKey).IsRequired().HasMaxLength(50);
                entity.Property(l => l.PrimaryMacAddress).HasMaxLength(17);
                entity.Property(l => l.HardwareFingerprint).HasMaxLength(64);
                entity.HasMany(l => l.LicensedDevices).WithOne(d => d.License).HasForeignKey(d => d.LicenseInfoId).OnDelete(DeleteBehavior.Cascade);
            });

            // LicenseDevice configuration
            modelBuilder.Entity<LicenseDevice>(entity =>
            {
                entity.HasIndex(d => d.MacAddress);
                entity.HasIndex(d => d.HardwareFingerprint);
                entity.HasIndex(d => d.LicenseInfoId);
                entity.Property(d => d.MacAddress).HasMaxLength(17);
                entity.Property(d => d.HardwareFingerprint).HasMaxLength(64);
                entity.Property(d => d.MachineName).HasMaxLength(100);
            });
        }
    }
}
