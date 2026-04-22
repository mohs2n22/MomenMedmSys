using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using MomenMedmSys.WPF.ViewModels.Base;

namespace MomenMedmSys.WPF.ViewModels
{
    /// <summary>
    /// ViewModel for the Services Documentation page.
    /// Provides detailed documentation for all 26 business services including
    /// method signatures, dependencies, usage examples, and compliance notes.
    /// </summary>
    public partial class ServicesDocViewModel : ViewModelBase
    {
        public ServicesDocViewModel()
        {
            Title = "Services Documentation";
            BuildServiceDocs();
        }

        public ObservableCollection<ServiceDocItem> AllServices { get; } = new();
        public ObservableCollection<ServiceCategoryGroup> CategoryGroups { get; } = new();

        private void BuildServiceDocs()
        {
            // ─── Asset Management (2) ───
            var assetMgmt = new ServiceCategoryGroup("🏥 Asset Management", "#3B82F6");
            CategoryGroups.Add(assetMgmt);

            AddService("DeviceService", "IDeviceService", "Asset Management",
                "Core service for the Device Register module. Manages the complete lifecycle of medical devices including procurement, installation, operation, and decommissioning.",
                new[]
                {
                    "GetAllDevicesAsync() — Retrieve every registered medical device",
                    "GetDeviceByIdAsync(id) — Fetch single device with full details",
                    "CreateDeviceAsync(device) — Register new device with barcode/GPS/RFID",
                    "UpdateDeviceAsync(device) — Modify device properties and status",
                    "DeleteDeviceAsync(id) — Remove device from registry",
                    "GetDevicesByDepartmentAsync(deptId) — Filter by hospital department",
                    "GetDevicesByStatusAsync(status) — Active/Maintenance/Decommissioned",
                    "GetDevicesByRiskClassAsync(riskClass) — Class I/II/III filtering",
                    "GetDevicesDueForMaintenanceAsync() — Overdue maintenance alerts",
                    "GetDevicesDueForCalibrationAsync() — Overdue calibration alerts",
                    "GetDevicesWithExpiringWarrantyAsync(days) — Warranty expiry warnings",
                    "GetTotalDeviceCountAsync() — Total registered devices",
                    "GetActiveDeviceCountAsync() — Currently operational devices",
                    "GetTotalAssetValueAsync() — Aggregate purchase price of all devices",
                    "GetDeviceTotalCostAsync(deviceId) — Total cost incl. maintenance"
                },
                new[] { "Device Register", "Dashboard", "Maintenance", "Reports" },
                "Device lifecycle tracking with barcode/RFID/GPS support. Integrates with Warranty Expiry alerts on the Dashboard.",
                "ISO 13485, FDA 21 CFR 820");

            AddService("DocumentService", "IDocumentService", "Asset Management",
                "Manages device-related documents including manuals, certificates, warranties, and specifications with version control and file attachment support.",
                new[]
                {
                    "GetAllDocumentsAsync() — All uploaded documents across devices",
                    "GetDocumentsByDeviceAsync(deviceId) — Documents for specific device",
                    "GetDocumentByIdAsync(id) — Fetch single document metadata",
                    "AddDocumentAsync(document) — Upload new document with metadata",
                    "UpdateDocumentAsync(document) — Update document version/info",
                    "DeleteDocumentAsync(id) — Remove document and file reference",
                    "GetDocumentsByTypeAsync(docType) — Filter by Manual/Certificate/Warranty",
                    "GetDeviceDocumentStatsAsync(deviceId) — Document count/size breakdown"
                },
                new[] { "Device Register", "Device Documents View" },
                "Supports version control for device manuals and compliance certificates.",
                "ISO 13485 Document Control");

            // ─── Maintenance (4) ───
            var maint = new ServiceCategoryGroup("🔧 Maintenance & Calibration", "#F59E0B");
            CategoryGroups.Add(maint);

            AddService("MaintenanceService", "IMaintenanceService", "Maintenance",
                "Handles preventive and corrective maintenance workflows — scheduling, cost tracking, technician assignment, and overdue detection.",
                new[]
                {
                    "GetAllRecordsAsync() — All maintenance records",
                    "GetRecordsByDeviceIdAsync(deviceId) — History for specific device",
                    "GetRecordByIdAsync(id) — Single maintenance record",
                    "CreateRecordAsync(record) — Log new maintenance event",
                    "UpdateRecordAsync(record) — Edit existing maintenance record",
                    "DeleteRecordAsync(id) — Remove maintenance record",
                    "GetOverdueMaintenanceAsync() — Past-due preventive maintenance",
                    "GetUpcomingMaintenanceAsync(days) — Scheduled within N days",
                    "GetRecordsByTypeAsync(type) — Preventive vs Corrective",
                    "GetRecordsByStatusAsync(status) — Scheduled/In-Progress/Completed",
                    "GetTotalMaintenanceCostAsync(deviceId) — Cumulative cost per device",
                    "GetOverdueCountAsync() — Count of overdue maintenance tasks",
                    "GetScheduledCountAsync() — Count of upcoming scheduled tasks"
                },
                new[] { "Maintenance View", "Dashboard", "Analytics", "Work Orders" },
                "Feeds MTBF/MTTR calculations to AnalyticsService. Creates WorkOrderService entries for corrective maintenance.",
                "JCI Preventive Maintenance Standards");

            AddService("CalibrationService", "ICalibrationService", "Maintenance",
                "Manages calibration records with as-found/as-left measurements, NIST-traceable standards tracking, and compliance reporting.",
                new[]
                {
                    "GetAllRecordsAsync() — All calibration records",
                    "GetRecordsByDeviceIdAsync(deviceId) — Calibration history per device",
                    "GetRecordByIdAsync(id) — Single calibration record",
                    "CreateRecordAsync(record) — Log calibration with measurements",
                    "UpdateRecordAsync(record) — Edit calibration data",
                    "DeleteRecordAsync(id) — Remove calibration record",
                    "GetOverdueCalibrationsAsync() — Past-due calibration tasks",
                    "GetUpcomingCalibrationsAsync(days) — Due within N days",
                    "GetRecordsByResultAsync(result) — Pass/Fail/As-Left-Within-Tolerance",
                    "GetOverdueCountAsync() — Count of overdue calibrations",
                    "GetPassCountAsync() — Total passing calibrations",
                    "GetFailCountAsync() — Total failing calibrations"
                },
                new[] { "Calibration View", "Dashboard", "Analytics", "Reports" },
                "Tracks as-found/as-left measurements with NIST-traceable reference standards.",
                "ISO 17025, NIST Traceability");

            AddService("ElectricalSafetyService", "IElectricalSafetyService", "Maintenance",
                "Manages electrical safety testing for medical devices — leakage current, ground bond, insulation resistance testing.",
                new[]
                {
                    "GetAllTestsAsync() — All electrical safety test records",
                    "GetTestsByDeviceIdAsync(deviceId) — Test history per device",
                    "GetTestByIdAsync(id) — Single test record",
                    "CreateTestAsync(test) — Record new safety test",
                    "UpdateTestAsync(test) — Edit test results",
                    "DeleteTestAsync(id) — Remove test record",
                    "GetOverdueTestsAsync() — Past-due safety tests",
                    "GetTestsDueSoonAsync(days) — Due within threshold",
                    "GetFailedTestsAsync() — All failed safety tests",
                    "GetOverdueCountAsync() — Count of overdue tests",
                    "GetPassCountAsync() — Count of passing tests",
                    "GetFailCountAsync() — Count of failing tests"
                },
                new[] { "Safety Tests View", "Dashboard", "Reports" },
                "Tracks electrical safety compliance per IEC 60601-1 standards.",
                "IEC 60601-1, NFPA 99");

            AddService("WorkOrderService", "IWorkOrderService", "Maintenance",
                "Complete work order lifecycle — creation from maintenance events, assignment to technicians/contractors, priority tracking, and closure.",
                new[]
                {
                    "GetAllWorkOrdersAsync() — All work orders",
                    "GetWorkOrdersByDeviceIdAsync(deviceId) — WO history per device",
                    "GetWorkOrderByIdAsync(id) — Single work order",
                    "CreateWorkOrderAsync(wo) — Create new work order",
                    "UpdateWorkOrderAsync(wo) — Update status/assignment/notes",
                    "DeleteWorkOrderAsync(id) — Remove work order",
                    "GetOpenWorkOrdersAsync() — Active/uncompleted work orders",
                    "GetOverdueWorkOrdersAsync() — Past-due work orders",
                    "GetWorkOrdersByStatusAsync(status) — Open/In-Progress/Closed",
                    "GetWorkOrdersByPriorityAsync(priority) — Low/Medium/High/Emergency",
                    "GetOpenWorkOrderCountAsync() — Count of open WOs",
                    "GetOverdueWorkOrderCountAsync() — Count of overdue WOs",
                    "GenerateWorkOrderNumberAsync() — Auto-generate unique WO number"
                },
                new[] { "Work Orders View", "Maintenance", "Dashboard" },
                "Auto-generated from corrective maintenance events. Supports technician and contractor assignment.",
                "");

            // ─── Inventory (1) ───
            var inventory = new ServiceCategoryGroup("📦 Inventory & Parts", "#10B981");
            CategoryGroups.Add(inventory);

            AddService("SparePartService", "ISparePartService", "Inventory",
                "Spare parts inventory management — stock level monitoring, usage tracking, reorder alerts, and consumption recording against maintenance events.",
                new[]
                {
                    "GetAllPartsAsync() — All spare parts in inventory",
                    "GetPartByIdAsync(id) — Single spare part record",
                    "CreatePartAsync(part) — Add new spare part to inventory",
                    "UpdatePartAsync(part) — Update stock levels/pricing",
                    "DeletePartAsync(id) — Remove spare part",
                    "GetLowStockPartsAsync() — Below minimum stock level",
                    "GetReorderNeededAsync() — At or below reorder point",
                    "GetCriticalPartsAsync() — Zero stock, critical for operations",
                    "GetUsageHistoryAsync(partId) — Usage log for specific part",
                    "GetTotalInventoryValueAsync() — Total value of all spare parts",
                    "GetLowStockCountAsync() — Count of low-stock items",
                    "UsePartAsync(partId, maintId, qty, notes) — Record part consumption"
                },
                new[] { "Spare Parts View", "Maintenance", "Reports" },
                "Integrates with maintenance records — parts consumed are linked to specific maintenance events.",
                "");

            // ─── Risk Management (1) ───
            var risk = new ServiceCategoryGroup("⚠️ Risk Management", "#EF4444");
            CategoryGroups.Add(risk);

            AddService("RiskService", "IRiskService", "Risk Management",
                "Risk incident management per ISO 14971 — severity/probability scoring, risk matrix calculation, incident tracking, and safety event logging.",
                new[]
                {
                    "GetAllIncidentsAsync() — All risk/safety incidents",
                    "GetIncidentsByDeviceIdAsync(deviceId) — Incidents per device",
                    "GetIncidentByIdAsync(id) — Single incident record",
                    "CreateIncidentAsync(incident) — Log new risk incident",
                    "UpdateIncidentAsync(incident) — Edit incident details/scoring",
                    "DeleteIncidentAsync(id) — Remove incident record",
                    "GetOpenIncidentsAsync() — Unresolved incidents",
                    "GetIncidentsByRiskLevelAsync(level) — Low/Medium/High/Critical",
                    "GetOpenIncidentCountAsync() — Count of open incidents",
                    "GetCriticalIncidentCountAsync() — Count of critical incidents"
                },
                new[] { "Risk Management View", "Dashboard", "Analytics", "Reports" },
                "Implements ISO 14971 risk matrix: Risk = Severity × Probability. Auto-calculates risk level.",
                "ISO 14971, JCI Safety Standards");

            // ─── Contracts & Procurement (2) ───
            var contracts = new ServiceCategoryGroup("📝 Contracts & Procurement", "#8B5CF6");
            CategoryGroups.Add(contracts);

            AddService("ServiceContractService", "IServiceContractService", "Contracts",
                "External service contract management — vendor agreements, SLA tracking, renewal alerts, and contract value analysis.",
                new[]
                {
                    "GetAllContractsAsync() — All service contracts",
                    "GetContractByIdAsync(id) — Single contract details",
                    "CreateContractAsync(contract) — Register new service contract",
                    "UpdateContractAsync(contract) — Edit contract terms",
                    "DeleteContractAsync(id) — Remove contract",
                    "GetActiveContractsAsync() — Currently valid contracts",
                    "GetExpiringSoonContractsAsync(days) — Expiring within threshold",
                    "GetExpiredContractsAsync() — Past-expiry contracts",
                    "GetActiveContractCountAsync() — Count of active contracts",
                    "GetExpiringSoonCountAsync(days) — Count expiring soon",
                    "GetTotalContractValueAsync() — Aggregate contract values"
                },
                new[] { "Service Contracts View", "Dashboard", "Reports" },
                "Tracks SLA compliance and renewal dates for external maintenance contracts.",
                "");

            AddService("ProcurementService", "IProcurementService", "Procurement",
                "Equipment procurement request management — request creation, approval workflows, technical evaluation, and purchase order tracking.",
                new[]
                {
                    "GetAllAsync() — All procurement requests",
                    "GetByIdAsync(id) — Single procurement request",
                    "CreateAsync(request) — Submit new procurement request",
                    "UpdateAsync(request) — Update request details",
                    "DeleteAsync(id) — Remove procurement request",
                    "GetByStatusAsync(status) — Draft/Pending/Approved/Rejected",
                    "GetPendingAsync() — Awaiting approval requests",
                    "GetApprovedAsync() — Approved for purchase",
                    "GetPendingCountAsync() — Count of pending requests",
                    "GetApprovedCountAsync() — Count of approved requests",
                    "GenerateRequestNumberAsync() — Auto-generate unique PR number",
                    "GetEvaluationsByRequestIdAsync(reqId) — Technical evaluations",
                    "CreateEvaluationAsync(evaluation) — Add technical evaluation",
                    "UpdateEvaluationAsync(evaluation) — Edit evaluation",
                    "DeleteEvaluationAsync(id) — Remove evaluation"
                },
                new[] { "Procurement View", "Dashboard", "Reports" },
                "Supports multi-stage approval workflow with technical evaluation scoring.",
                "");

            // ─── Staff & Training (3) ───
            var staff = new ServiceCategoryGroup("👥 Staff & Training", "#06B6D4");
            CategoryGroups.Add(staff);

            AddService("StaffService", "IStaffService", "Staff & Training",
                "Basic staff member management — employee records, department association, and active staff queries.",
                new[]
                {
                    "GetAllStaffAsync() — All staff members",
                    "GetStaffByIdAsync(id) — Single staff member",
                    "CreateStaffAsync(staff) — Register new staff member",
                    "UpdateStaffAsync(staff) — Update staff details",
                    "DeleteStaffAsync(id) — Remove staff member",
                    "GetStaffByDepartmentAsync(dept) — Filter by department",
                    "GetActiveStaffAsync() — Currently employed staff"
                },
                new[] { "Staff View", "Admin Panel", "Work Orders" },
                "Used by Staff & Training module for general staff CRUD operations.",
                "");

            AddService("StaffManagementService", "IStaffManagementService", "Staff & Training",
                "Extended staff administration for the Admin Control Panel — role-based queries, account management, password operations, and permission controls.",
                new[]
                {
                    "GetAllStaffAsync() — All staff (admin view)",
                    "GetStaffByRoleAsync(role) — Filter by role type",
                    "GetActiveStaffAsync() — Active staff list",
                    "GetStaffByDepartmentAsync(dept) — Department filter",
                    "GetStaffByIdAsync(id) — Single staff record",
                    "GetStaffByUsernameAsync(username) — Lookup by login name",
                    "CreateStaffAsync(staff) — Create staff account",
                    "UpdateStaffAsync(staff) — Update staff details",
                    "DeleteStaffAsync(id) — Remove staff account",
                    "ResetPasswordAsync(staffId, hash) — Admin password reset",
                    "ToggleAccountLockAsync(staffId, locked) — Lock/unlock account",
                    "UpdateLastLoginAsync(staffId) — Record login timestamp",
                    "GetActiveAccountCountAsync() — Active user count",
                    "GetLockedAccountCountAsync() — Locked user count",
                    "GetAdministratorsAsync() — All admin users",
                    "GetHardwareTechniciansAsync() — All technician users",
                    "GetReportWritersAsync() — All report writer users",
                    "GetPhysiciansAsync() — All physician users",
                    "GetNursesAsync() — All nurse users"
                },
                new[] { "Admin Control Panel", "Authentication" },
                "Admin-only service. Manages account lifecycle, permissions, and role-based access.",
                "");

            AddService("TrainingService", "ITrainingService", "Staff & Training",
                "Staff training and certification management — training records, expiry tracking, per-staff/device queries.",
                new[]
                {
                    "GetAllTrainingRecordsAsync() — All training certifications",
                    "GetTrainingByStaffIdAsync(staffId) — Training history per staff",
                    "GetTrainingByDeviceIdAsync(deviceId) — Device-specific training",
                    "CreateTrainingAsync(record) — Record new training/certification",
                    "UpdateTrainingAsync(record) — Update training details",
                    "DeleteTrainingAsync(id) — Remove training record",
                    "GetExpiredTrainingAsync() — Expired certifications",
                    "GetExpiringSoonTrainingAsync(days) — Expiring within threshold"
                },
                new[] { "Staff View", "Dashboard", "Reports" },
                "Tracks certification expiry dates and alerts for renewal.",
                "JCI Staff Competency Standards");

            // ─── Network Monitoring (1) ───
            var network = new ServiceCategoryGroup("🌐 Network Monitoring", "#6366F1");
            CategoryGroups.Add(network);

            AddService("NetworkDiscoveryService", "INetworkDiscoveryService", "Network Monitoring",
                "ICMP ping-based network device discovery and remote monitoring — scanning, response time measurement, status tracking, and remote action execution.",
                new[]
                {
                    "GetAllNetworkDevicesAsync() — All registered network devices",
                    "GetDeviceByIdAsync(id) — Single network device",
                    "AddDeviceAsync(device) — Register network device",
                    "UpdateDeviceAsync(device) — Update device config",
                    "DeleteDeviceAsync(id) — Remove network device",
                    "DiscoverNetworkAsync(subnet) — Scan subnet for devices",
                    "PingDeviceAsync(ipAddress) — ICMP ping test",
                    "GetResponseTimeAsync(ipAddress) — Ping response time (ms)",
                    "CheckDeviceStatusAsync(device) — Online/Offline/Warning status",
                    "RefreshAllDeviceStatusesAsync() — Bulk status refresh",
                    "ExecuteRemoteActionAsync(deviceId, action) — Remote command execution",
                    "GetActionLogsAsync(deviceId) — Remote action history",
                    "GetOnlineCountAsync() — Reachable devices",
                    "GetOfflineCountAsync() — Unreachable devices",
                    "GetWarningCountAsync() — Devices with warnings"
                },
                new[] { "Network Devices View", "Dashboard" },
                "Uses ICMP ping for health checks. Supports remote actions: reboot, shutdown, status query.",
                "");

            // ─── Analytics (2) ───
            var analytics = new ServiceCategoryGroup("📊 Analytics & Reporting", "#EC4899");
            CategoryGroups.Add(analytics);

            AddService("DashboardService", "IDashboardService", "Analytics",
                "Real-time dashboard statistics — device counts, maintenance/calibration status, work order counts, and notification summaries.",
                new[]
                {
                    "GetDashboardStatsAsync() — Complete dashboard data package"
                },
                new[] { "Dashboard View" },
                "Aggregates data from all services into a single DashboardStats object for the home screen.",
                "");

            AddService("AnalyticsService", "IAnalyticsService", "Analytics",
                "Advanced KPI calculations and trend analysis — MTBF, MTTR, equipment availability, cost analysis, and failure reporting.",
                new[]
                {
                    "GetEquipmentAvailabilityAsync() — Uptime percentage",
                    "GetMTBFAsync() — Mean Time Between Failures (hours)",
                    "GetMTTRAsync() — Mean Time To Repair (hours)",
                    "GetMaintenanceCompletionRateAsync() — Completed vs Scheduled",
                    "GetCalibrationComplianceAsync() — On-time calibration rate",
                    "GetCostPerDeviceAsync() — Average maintenance cost per device",
                    "GetWorkOrderStatusDistributionAsync() — Status breakdown",
                    "GetMaintenanceByMonthAsync(months) — Monthly trend data",
                    "GetDeviceStatusDistributionAsync() — Active/Maintenance/Offline",
                    "GetDepartmentCostComparisonAsync() — Cost by department",
                    "GetRiskIncidentTrendAsync(months) — Monthly incident trend",
                    "GetTopFailingEquipmentAsync(count) — Highest failure rate devices",
                    "GetWarrantyExpiryTimelineAsync() — Upcoming warranty expirations"
                },
                new[] { "Reports View", "Dashboard", "Analytics Charts" },
                "Powers all charts and KPI cards on the Dashboard and Reports pages. Uses LiveCharts for visualization.",
                "");

            // ─── Security (3) ───
            var security = new ServiceCategoryGroup("🔐 Security & Access", "#DC2626");
            CategoryGroups.Add(security);

            AddService("AuthService", "IAuthService", "Security",
                "User authentication and session management — BCrypt password hashing, login/logout, password operations, and account lockout.",
                new[]
                {
                    "AuthenticateAsync(username, password) — Validate credentials",
                    "LogoutAsync(userId) — End user session",
                    "ChangePasswordAsync(userId, old, new) — User password change",
                    "ResetPasswordAsync(userId, new) — Admin password reset",
                    "LockAccountAsync(userId) — Lock user account",
                    "UnlockAccountAsync(userId) — Unlock and reset failed attempts",
                    "GetAllUsersAsync() — All registered users",
                    "CreateUserAsync(user, password) — Register new user",
                    "UpdateUserAsync(user) — Update user details",
                    "DeleteUserAsync(userId) — Soft-delete user account",
                    "GetCurrentUserAsync(userId) — Get authenticated user",
                    "IsPasswordValid(password, hash) — BCrypt verification",
                    "HashPassword(password) — BCrypt hash generation (work factor 12)"
                },
                new[] { "Login View", "Admin Panel", "Session Management" },
                "Uses BCrypt with work factor 12 for password hashing. Supports session tracking and account lockout.",
                "OWASP Password Storage Guidelines");

            AddService("AuditService", "IAuditService", "Security",
                "Comprehensive audit trail logging — records every data change with entity type, action, user, timestamp, before/after values, and Excel export.",
                new[]
                {
                    "LogAsync(entity, id, action, user, old, new) — Record audit entry",
                    "GetAuditLogsAsync(filters) — Filtered audit log query",
                    "GetEntityHistoryAsync(entityType, entityId) — Full change history",
                    "GetUserActivityAsync(userId, dates) — User activity timeline",
                    "GetRecentActivityAsync(count) — Latest N audit entries",
                    "ExportAuditLogsAsync(filePath) — Excel export with filters",
                    "GetTotalAuditLogCountAsync() — Total audit entries"
                },
                new[] { "Admin Panel", "Audit Log View", "Reports" },
                "Every create/update/delete operation across all services triggers an audit log entry.",
                "FDA 21 CFR Part 11, JCI Audit Standards");

            AddService("NotificationService", "INotificationService", "Security",
                "In-app notification management — creation, delivery, read/unread tracking, and system alert generation.",
                new[]
                {
                    "CreateNotificationAsync(notification) — Send notification to user",
                    "GetUnreadCountAsync(userId) — Unread notification count",
                    "GetNotificationsAsync(userId, isRead, count) — User notification list",
                    "MarkAsReadAsync(notificationId) — Mark single as read",
                    "MarkAllAsReadAsync(userId) — Bulk mark-as-read",
                    "DeleteNotificationAsync(notificationId) — Remove notification",
                    "GenerateSystemAlertsAsync() — Auto-generate alerts from thresholds",
                    "GetNotificationsSummaryAsync(userId) — Summary by type/priority"
                },
                new[] { "Notification Panel", "Dashboard", "All Modules" },
                "Auto-generates alerts for overdue maintenance, expiring warranties, low stock, and risk incidents.",
                "");

            // ─── Administration (4) ───
            var admin = new ServiceCategoryGroup("⚙️ Administration", "#64748B");
            CategoryGroups.Add(admin);

            AddService("DepartmentService", "IDepartmentService", "Administration",
                "Hospital department management — CRUD operations, active department queries, and device/staff association.",
                new[]
                {
                    "GetAllAsync() — All departments",
                    "GetByIdAsync(id) — Single department",
                    "CreateAsync(department) — Add new department",
                    "UpdateAsync(department) — Update department details",
                    "DeleteAsync(id) — Remove department",
                    "GetActiveAsync() — Currently active departments",
                    "GetActiveCountAsync() — Count of active departments"
                },
                new[] { "Departments View", "Device Register", "Staff" },
                "Departments are used to categorize devices, assign staff, and filter work orders.",
                "");

            AddService("SupplierService", "ISupplierService", "Administration",
                "Supplier/vendor management — CRUD operations, approved supplier tracking, and rating-based filtering.",
                new[]
                {
                    "GetAllAsync() — All suppliers",
                    "GetByIdAsync(id) — Single supplier",
                    "CreateAsync(supplier) — Register new supplier",
                    "UpdateAsync(supplier) — Update supplier details",
                    "DeleteAsync(id) — Remove supplier",
                    "GetApprovedAsync() — Approved vendors only",
                    "GetByRatingAsync(minRating) — Filter by minimum rating",
                    "GetApprovedCountAsync() — Count of approved suppliers"
                },
                new[] { "Suppliers View", "Procurement", "Service Contracts" },
                "Used in procurement workflows and device manufacturer tracking.",
                "");

            AddService("LicenseService", "ILicenseService", "Administration",
                "License activation and validation — key generation, format validation, hardware fingerprinting, device registration, and status reporting.",
                new[]
                {
                    "GetCurrentLicenseAsync() — Active license details",
                    "ActivateAsync(licenseKey) — Activate with license key",
                    "ValidateAsync() — Validate current license state",
                    "RegisterCurrentDeviceAsync() — Register this machine",
                    "GenerateLicenseKey(type) — Generate key (3M/1Y/Lifetime)",
                    "ValidateKeyFormat(key) — Verify key structure",
                    "RemoveDeviceAsync(licenseDeviceId) — Unregister device",
                    "GetRemainingSlotsAsync() — Available device slots",
                    "IsLifetimeLicenseAsync() — Check if perpetual license",
                    "GetLicenseStatusTextAsync() — Human-readable status"
                },
                new[] { "Admin Panel", "License Tab" },
                "Uses hardware fingerprinting (MAC address + machine ID) for license binding.",
                "");

            AddService("HardwareInfoService", "IHardwareInfoService", "Administration",
                "Hardware identification — MAC address retrieval, hardware fingerprint generation, and machine identification for license binding.",
                new[]
                {
                    "GetMacAddress() — Primary network adapter MAC",
                    "GetHardwareFingerprint() — Composite hardware hash",
                    "GetMachineName() — Computer hostname",
                    "GetMachineIdentifier() — Unique machine ID"
                },
                new[] { "LicenseService", "Admin Panel" },
                "Used by LicenseService to generate hardware-bound license keys.",
                "");

            AddService("DatabaseBackupService", "IDatabaseBackupService", "Administration",
                "Database backup and restore — full/compressed backup, scheduled backup, restore, cleanup, validation, and disk space management.",
                new[]
                {
                    "CreateBackupAsync(path) — Full database copy",
                    "CreateTimestampedBackupAsync(path) — Dated backup file",
                    "CompressBackupAsync(filePath) — ZIP compression",
                    "RestoreBackupAsync(backupPath, dbPath) — Restore from backup",
                    "CleanupOldBackupsAsync(path, retentionDays) — Delete old backups",
                    "ValidateBackupAsync(backupPath) — Integrity check",
                    "GetBackupHistoryAsync(path) — List of all backups",
                    "GetDatabaseInfoAsync() — Current database file info",
                    "GetAvailableDiskSpaceAsync(path) — Free disk space"
                },
                new[] { "Admin Panel", "System Tab", "Database Backup View" },
                "Supports compression, validation, and retention policies. Atomic file copy for safety.",
                "");

            // Build flat list
            foreach (var group in CategoryGroups)
            {
                // Services are already added to AllServices via AddService
            }

            StatusMessage = $"Loaded documentation for {AllServices.Count} services across {CategoryGroups.Count} categories";
        }

        private void AddService(string name, string interfaceName, string category,
            string description, string[] methods, string[] usedBy, string notes, string compliance)
        {
            var item = new ServiceDocItem
            {
                Name = name,
                InterfaceName = interfaceName,
                Category = category,
                Description = description,
                Methods = methods,
                UsedBy = usedBy,
                Notes = notes,
                Compliance = compliance
            };
            AllServices.Add(item);

            // Add to category group
            var group = CategoryGroups.FirstOrDefault(g => g.Name.Contains(category.Split(' ')[0]));
            group?.Services.Add(item);
        }
    }

    public class ServiceCategoryGroup : ObservableObject
    {
        public string Name { get; }
        public string Color { get; }
        public ObservableCollection<ServiceDocItem> Services { get; } = new();

        public ServiceCategoryGroup(string name, string color)
        {
            Name = name;
            Color = color;
        }
    }

    public partial class ServiceDocItem : ObservableObject
    {
        [ObservableProperty] private string _name = string.Empty;
        [ObservableProperty] private string _interfaceName = string.Empty;
        [ObservableProperty] private string _category = string.Empty;
        [ObservableProperty] private string _description = string.Empty;
        [ObservableProperty] private string[] _methods = Array.Empty<string>();
        [ObservableProperty] private string[] _usedBy = Array.Empty<string>();
        [ObservableProperty] private string _notes = string.Empty;
        [ObservableProperty] private string _compliance = string.Empty;
    }
}
