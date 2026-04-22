using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MomenMedmSys.Services;
using MomenMedmSys.WPF.Services;
using MomenMedmSys.WPF.ViewModels.Base;

namespace MomenMedmSys.WPF.ViewModels
{
    /// <summary>
    /// ViewModel for the About Us page/dialog.
    /// Displays application info, mission, team, contact details, and a full catalog
    /// of all 26 business services provided by MomenMedmSys.
    /// </summary>
    public partial class AboutUsViewModel : ViewModelBase
    {
        private readonly IDatabaseBackupService _backupService;
        private readonly IAuditService _auditService;
        private readonly IServiceProvider _serviceProvider;
        private MainViewModel? _mainViewModel;

        public AboutUsViewModel(IServiceProvider serviceProvider, IDatabaseBackupService backupService, IAuditService auditService)
        {
            _serviceProvider = serviceProvider;
            _backupService = backupService;
            _auditService = auditService;
            Title = "About MomenMedmSys";
            LoadSystemInfoCommand.Execute(null);
        }

        public void SetMainViewModel(MainViewModel mainVm) => _mainViewModel = mainVm;

        // Application Info
        [ObservableProperty] private string _appName = "Momen Systems Co.";
        [ObservableProperty] private string _version = "1.0.0";
        [ObservableProperty] private string _copyright = "© 2026 Momen Systems Co. — Wad Madani, Sudan. All Rights Reserved.";
        [ObservableProperty] private string _buildDate = "April 2026";
        [ObservableProperty] private string _fullDescription =
            "Momen Systems Co. presents a comprehensive Medical Equipment Management System (MEMS) designed for hospitals " +
            "and healthcare facilities. It digitizes the entire lifecycle of medical equipment — from procurement " +
            "planning and inventory tracking to preventive/corrective maintenance, calibration, risk management, " +
            "and compliance reporting.\n\n" +
            "Built on .NET 8.0 with WPF, EF Core, and SQLite, it provides a modern, self-contained desktop " +
            "application that replaces paper-based equipment tracking with a fully digital CMMS solution.";

        // Mission Statement
        [ObservableProperty] private string _missionTitle = "Our Mission";
        [ObservableProperty] private string _missionText =
            "To empower healthcare facilities with reliable, compliant, and efficient medical equipment " +
            "management — ensuring patient safety, regulatory compliance (JCI, ISO 13485, ISO 14971, FDA), " +
            "and optimal equipment uptime through intelligent digital transformation.";

        // Compliance Standards
        public ObservableCollection<string> ComplianceStandards { get; } = new()
        {
            "JCI (Joint Commission International)",
            "ISO 13485 — Medical Devices QMS",
            "ISO 14971 — Risk Management",
            "FDA 21 CFR Part 820 — Quality System Regulation",
            "NIST-Traceable Calibration Standards"
        };

        // Technology Stack
        public ObservableCollection<TechItem> TechnologyStack { get; } = new()
        {
            new TechItem { Name = ".NET 8.0", Category = "Framework", Icon = "🖥️" },
            new TechItem { Name = "WPF", Category = "UI Layer", Icon = "🎨" },
            new TechItem { Name = "CommunityToolkit.Mvvm 8.4.2", Category = "MVVM", Icon = "📐" },
            new TechItem { Name = "EF Core 8.0.2", Category = "ORM", Icon = "🗃️" },
            new TechItem { Name = "SQLite", Category = "Database", Icon = "📦" },
            new TechItem { Name = "LiveCharts 2.0", Category = "Charts", Icon = "📊" },
            new TechItem { Name = "BCrypt.Net-Next 4.0.3", Category = "Security", Icon = "🔐" },
            new TechItem { Name = "ClosedXML 0.104.2", Category = "Excel Export", Icon = "📋" }
        };

        // Services Catalog — 26 business services
        public ObservableCollection<ServiceItem> ServicesCatalog { get; } = new()
        {
            // Asset & Device Management
            new ServiceItem { Category = "Asset Management", Name = "DeviceService", Icon = "🏥",
                Description = "Full CRUD for medical devices with barcode/RFID/GPS tracking, status queries, warranty expiry alerts, and device lifecycle management." },
            new ServiceItem { Category = "Asset Management", Name = "DocumentService", Icon = "📄",
                Description = "Device document management for manuals, certificates, and warranties with version control and file attachment support." },

            // Maintenance & Calibration
            new ServiceItem { Category = "Maintenance", Name = "MaintenanceService", Icon = "🔧",
                Description = "Preventive and corrective maintenance scheduling with cost tracking, work order generation, and technician assignment." },
            new ServiceItem { Category = "Maintenance", Name = "CalibrationService", Icon = "📏",
                Description = "Calibration record management with as-found/as-left measurements, NIST-traceable standards tracking, and overdue alerts." },
            new ServiceItem { Category = "Maintenance", Name = "ElectricalSafetyService", Icon = "⚡",
                Description = "Electrical safety test management for medical devices, recording test results and compliance verification." },
            new ServiceItem { Category = "Maintenance", Name = "WorkOrderService", Icon = "📋",
                Description = "Complete work order lifecycle management — creation, assignment, tracking, and closure with technician/contractor assignment." },

            // Inventory & Parts
            new ServiceItem { Category = "Inventory", Name = "SparePartService", Icon = "📦",
                Description = "Spare parts inventory management with min/max stock level alerts, usage tracking, and reorder notifications." },

            // Risk & Compliance
            new ServiceItem { Category = "Risk Management", Name = "RiskService", Icon = "⚠️",
                Description = "Risk incident management per ISO 14971 with severity/probability scoring, risk matrix calculation, and safety incident tracking." },

            // Contracts & Procurement
            new ServiceItem { Category = "Contracts", Name = "ServiceContractService", Icon = "📝",
                Description = "External service contract management with SLA tracking, renewal alerts, and vendor performance monitoring." },
            new ServiceItem { Category = "Procurement", Name = "ProcurementService", Icon = "🛒",
                Description = "Equipment procurement request management with technical evaluation, approval workflows, and purchase order tracking." },

            // Staff & Training
            new ServiceItem { Category = "Staff & Training", Name = "StaffService", Icon = "👤",
                Description = "Staff member management with employee records, role assignment, department association, and contact information." },
            new ServiceItem { Category = "Staff & Training", Name = "StaffManagementService", Icon = "👥",
                Description = "Extended staff administration including account locking, password resets, permission management, and bulk operations." },
            new ServiceItem { Category = "Staff & Training", Name = "TrainingService", Icon = "🎓",
                Description = "Staff training records management with certification tracking, expiry alerts, and competency verification." },

            // Network & Monitoring
            new ServiceItem { Category = "Network Monitoring", Name = "NetworkDiscoveryService", Icon = "🌐",
                Description = "ICMP ping-based network device discovery and remote monitoring for connected medical equipment." },

            // Analytics & Reporting
            new ServiceItem { Category = "Analytics", Name = "DashboardService", Icon = "📊",
                Description = "Dashboard statistics and alert aggregation — provides real-time KPIs, pending tasks, and notification summaries." },
            new ServiceItem { Category = "Analytics", Name = "AnalyticsService", Icon = "📈",
                Description = "Advanced KPI calculations including MTBF (Mean Time Between Failures), MTTR (Mean Time To Repair), equipment utilization, and cost analysis." },

            // Security & Access
            new ServiceItem { Category = "Security", Name = "AuthService", Icon = "🔐",
                Description = "User authentication with BCrypt password hashing, session tracking, account lockout, and role-based access control." },
            new ServiceItem { Category = "Security", Name = "AuditService", Icon = "📜",
                Description = "Comprehensive audit trail logging — records every data change with user, timestamp, before/after values, and CSV export." },
            new ServiceItem { Category = "Security", Name = "NotificationService", Icon = "🔔",
                Description = "In-app notification system with priority levels, type categorization, and delivery to relevant users." },

            // Administration
            new ServiceItem { Category = "Administration", Name = "DepartmentService", Icon = "🏢",
                Description = "Department CRUD operations — manage hospital departments and associate them with devices, staff, and work orders." },
            new ServiceItem { Category = "Administration", Name = "SupplierService", Icon = "🤝",
                Description = "Supplier/vendor management with contact details, performance tracking, and association to devices and contracts." },
            new ServiceItem { Category = "Administration", Name = "LicenseService", Icon = "🔑",
                Description = "License activation and validation with hardware fingerprinting, key generation, and expiry management." },
            new ServiceItem { Category = "Administration", Name = "HardwareInfoService", Icon = "🖧",
                Description = "MAC address and hardware fingerprint generation for license binding and device identification." },
            new ServiceItem { Category = "Administration", Name = "DatabaseBackupService", Icon = "💾",
                Description = "Database backup and restore with compression, scheduled backups, retention policies, and integrity validation." }
        };

        // Team Members
        public ObservableCollection<TeamMember> TeamMembers { get; } = new()
        {
            new TeamMember { Name = "Momen", Role = "Lead Developer & Architect", Avatar = "👨‍💻", Bio = "Designed and built the entire MomenMedmSys platform — architecture, development, and deployment." },
            new TeamMember { Name = "Development Team", Role = "Engineering & QA", Avatar = "👥", Bio = "Contributing engineers, testers, and domain experts who shaped the product." },
            new TeamMember { Name = "Healthcare Advisors", Role = "Domain Experts", Avatar = "🩺", Bio = "Biomedical engineers and hospital staff who provided requirements and validation." }
        };

        // Contact Information
        [ObservableProperty] private string _contactEmail = "MOHS2N@YAHOO.COM";
        [ObservableProperty] private string _contactPhone = "+249 124 349 024";
        [ObservableProperty] private string _contactWebsite = "https://www.momensystems.com";
        [ObservableProperty] private string _contactAddress = "Momen Systems Company";

        // System Info (loaded dynamically)
        [ObservableProperty] private string _databaseInfo = "Loading...";
        [ObservableProperty] private string _databaseSize = "Loading...";
        [ObservableProperty] private string _totalAuditLogs = "Loading...";
        [ObservableProperty] private string _totalDevices = "Loading...";
        [ObservableProperty] private string _totalStaff = "Loading...";
        [ObservableProperty] private string _totalBackups = "Loading...";
        [ObservableProperty] private string _systemUptime = "Loading...";
        [ObservableProperty] private string _memoryUsage = "Loading...";

        // Links
        [ObservableProperty] private string _licenseAgreementUrl = "https://www.momenmedmsys.com/license";
        [ObservableProperty] private string _privacyPolicyUrl = "https://www.momenmedmsys.com/privacy";
        [ObservableProperty] private string _documentationUrl = "https://www.momenmedmsys.com/docs";

        [RelayCommand]
        private void LoadSystemInfo()
        {
            try
            {
                var process = Process.GetCurrentProcess();
                var memoryMb = process.WorkingSet64 / (1024.0 * 1024.0);
                MemoryUsage = $"{memoryMb:F1} MB";

                var uptime = DateTime.Now - process.StartTime;
                SystemUptime = uptime.TotalDays > 1
                    ? $"{(int)uptime.TotalDays}d {uptime.Hours}h {uptime.Minutes}m"
                    : $"{uptime.Hours}h {uptime.Minutes}m";

                _ = LoadDatabaseInfoAsync();
            }
            catch { }
        }

        private async System.Threading.Tasks.Task LoadDatabaseInfoAsync()
        {
            try
            {
                var dbInfo = await _backupService.GetDatabaseInfoAsync();
                DatabaseInfo = Path.GetFileName(dbInfo?.FilePath ?? "Unknown");
                DatabaseSize = FormatFileSize(dbInfo?.FileSizeBytes ?? 0);
                TotalAuditLogs = (await _auditService.GetTotalAuditLogCountAsync()).ToString();
            }
            catch { DatabaseInfo = "Not available"; DatabaseSize = "N/A"; TotalAuditLogs = "N/A"; }
        }

        // These are set by the caller after construction (from DeviceService / StaffService)
        public void SetDeviceCount(int count) => TotalDevices = count.ToString();
        public void SetStaffCount(int count) => TotalStaff = count.ToString();
        public void SetBackupCount(int count) => TotalBackups = count.ToString();

        [RelayCommand]
        private void OpenUrl(string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
        }

        [RelayCommand]
        private void OpenEmailClient()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = $"mailto:{ContactEmail}",
                UseShellExecute = true
            });
        }

        [RelayCommand]
        private void NavigateToServicesDoc()
        {
            if (_mainViewModel != null && _serviceProvider != null)
            {
                var vm = (ServicesDocViewModel)_serviceProvider.GetRequiredService(typeof(ServicesDocViewModel));
                _mainViewModel.NavigateTo(vm);
            }
        }

        private static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1) { order++; len /= 1024; }
            return $"{len:0.##} {sizes[order]}";
        }
    }

    // Supporting model classes
    public partial class TechItem : ObservableObject
    {
        [ObservableProperty] private string _name = string.Empty;
        [ObservableProperty] private string _category = string.Empty;
        [ObservableProperty] private string _icon = string.Empty;
    }

    public partial class ServiceItem : ObservableObject
    {
        [ObservableProperty] private string _category = string.Empty;
        [ObservableProperty] private string _name = string.Empty;
        [ObservableProperty] private string _icon = string.Empty;
        [ObservableProperty] private string _description = string.Empty;
    }

    public partial class TeamMember : ObservableObject
    {
        [ObservableProperty] private string _name = string.Empty;
        [ObservableProperty] private string _role = string.Empty;
        [ObservableProperty] private string _avatar = string.Empty;
        [ObservableProperty] private string _bio = string.Empty;
    }
}
