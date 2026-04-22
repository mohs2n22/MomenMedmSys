using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Data;
using MomenMedmSys.Services;
using MomenMedmSys.WPF.Services;
using MomenMedmSys.WPF.ViewModels.Base;

namespace MomenMedmSys.WPF.ViewModels
{
    public partial class AdminControlPanelViewModel : ViewModelBase
    {
        private readonly IStaffManagementService _staffService;
        private readonly ILicenseService _licenseService;
        private readonly IDatabaseBackupService _backupService;
        private readonly IAuditService _auditService;
        private readonly IDeviceService _deviceService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CurrentUserService _currentUserService;
        private readonly IDialogService _dialogService;
        private System.Threading.Timer? _performanceTimer;

        public AdminControlPanelViewModel(IStaffManagementService staffService, ILicenseService licenseService,
            IDatabaseBackupService backupService, IAuditService auditService, IDeviceService deviceService,
            IUnitOfWork unitOfWork, CurrentUserService currentUserService, IDialogService dialogService)
        {
            _staffService = staffService;
            _licenseService = licenseService;
            _backupService = backupService;
            _auditService = auditService;
            _deviceService = deviceService;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _dialogService = dialogService;
            Title = "Admin Control Panel";
            LoadAllCommand.Execute(null);
            StartPerformanceMonitoring();
        }

        private void StartPerformanceMonitoring()
        {
            _performanceTimer = new System.Threading.Timer(async _ => await UpdatePerformanceMetrics(), null, 5000, 5000);
        }

        private async Task UpdatePerformanceMetrics()
        {
            try
            {
                var process = System.Diagnostics.Process.GetCurrentProcess();
                var memoryMb = process.WorkingSet64 / (1024.0 * 1024.0);
                MemoryUsage = $"{memoryMb:F1} MB";
                
                var cpuTime = process.TotalProcessorTime;
                var elapsed = DateTime.Now - process.StartTime;
                var cpuPercent = Math.Min(100, (cpuTime.TotalMilliseconds / elapsed.TotalMilliseconds) * 100);
                CpuUsage = $"{cpuPercent:F0}%";

                // Count active sessions
                var activeSessions = await _unitOfWork.UserSessions.FindAsync(s => s.LogoutTime == null);
                ActiveSessions = activeSessions.Count();
            }
            catch { }
        }

        // User lists by type
        public ObservableCollection<StaffMember> AllStaff { get; } = new();
        public ObservableCollection<StaffMember> Administrators { get; } = new();
        public ObservableCollection<StaffMember> HardwareTechnicians { get; } = new();
        public ObservableCollection<StaffMember> ReportWriters { get; } = new();
        public ObservableCollection<StaffMember> Physicians { get; } = new();
        public ObservableCollection<StaffMember> Nurses { get; } = new();

        // Filtered list for search
        public ObservableCollection<StaffMember> FilteredStaff { get; } = new();

        // Search
        [ObservableProperty] private string _searchText = string.Empty;

        partial void OnSearchTextChanged(string value)
        {
            ApplySearchFilter();
        }

        private void ApplySearchFilter()
        {
            FilteredStaff.Clear();
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                foreach (var s in AllStaff) FilteredStaff.Add(s);
            }
            else
            {
                var query = SearchText.ToLower();
                foreach (var s in AllStaff)
                {
                    if (s.FullName.ToLower().Contains(query) ||
                        s.Username.ToLower().Contains(query) ||
                        s.Email.ToLower().Contains(query) ||
                        s.Department.ToLower().Contains(query) ||
                        s.EmployeeId.ToLower().Contains(query) ||
                        s.Role.ToString().ToLower().Contains(query))
                    {
                        FilteredStaff.Add(s);
                    }
                }
            }
        }

        // Stats
        [ObservableProperty] private int _totalStaff;
        [ObservableProperty] private int _activeAccounts;
        [ObservableProperty] private int _lockedAccounts;
        [ObservableProperty] private int _adminCount;
        [ObservableProperty] private int _technicianCount;
        [ObservableProperty] private int _reportWriterCount;
        [ObservableProperty] private int _physicianCount;
        [ObservableProperty] private int _nurseCount;

        // Selected items
        [ObservableProperty] private StaffMember? _selectedStaff;
        [ObservableProperty] private int _activeTabIndex;

        // Tab switching commands
        [RelayCommand] private void SwitchToTab(int tabIndex) => ActiveTabIndex = tabIndex;

        // Form fields
        [ObservableProperty] private string _firstName = string.Empty;
        [ObservableProperty] private string _lastName = string.Empty;
        [ObservableProperty] private string _email = string.Empty;
        [ObservableProperty] private string _phone = string.Empty;
        [ObservableProperty] private string _username = string.Empty;
        [ObservableProperty] private string _password = string.Empty;
        [ObservableProperty] private StaffRole _selectedRole;
        [ObservableProperty] private string _department = string.Empty;
        [ObservableProperty] private string _jobTitle = string.Empty;
        [ObservableProperty] private string _specialization = string.Empty;
        [ObservableProperty] private bool _canManageDevices;
        [ObservableProperty] private bool _canManageMaintenance;
        [ObservableProperty] private bool _canManageCalibration;
        [ObservableProperty] private bool _canViewReports;
        [ObservableProperty] private bool _canAccessAdminPanel;
        [ObservableProperty] private bool _canManageSpareParts;
        [ObservableProperty] private bool _canManageNetworkDevices;
        [ObservableProperty] private bool _canManageStaff;
        
        // Form display
        [ObservableProperty] private string _formTitleText = "Add New Staff";
        [ObservableProperty] private string _formSubtitle = "Create a new staff account";
        [ObservableProperty] private string _formIcon = "➕";

        // License generation
        [ObservableProperty] private string _generatedLicenseKey = string.Empty;
        [ObservableProperty] private string _licenseStatusText = "Loading...";
        [ObservableProperty] private int _selectedLicenseTypeIndex = 0; // 0=3M, 1=1Y, 2=Lifetime
        [ObservableProperty] private int _totalLicenses;
        [ObservableProperty] private int _activeLicenses;
        [ObservableProperty] private string _updateStatus = string.Empty;
        [ObservableProperty] private bool _isUpdateInProgress;

        // Distribution package generation
        [ObservableProperty] private string _distributionStatus = string.Empty;
        [ObservableProperty] private bool _isDistributionInProgress;

        // Hospital / Institution Information
        [ObservableProperty] private string _hospitalName = string.Empty;
        [ObservableProperty] private string _administratorName = string.Empty;
        [ObservableProperty] private string _inputLicenseNumber = string.Empty;
        [ObservableProperty] private string _hospitalInfoStatus = string.Empty;

        // Current User (System Administrator)
        [ObservableProperty] private string _currentUserName = string.Empty;

        // System management
        [ObservableProperty] private string _systemVersion = "v1.0.0";
        [ObservableProperty] private string _databaseInfo = "Loading...";
        [ObservableProperty] private string _databaseSize = "Loading...";
        [ObservableProperty] private string _diskSpace = "Loading...";
        [ObservableProperty] private string _systemUptime = "Loading...";
        [ObservableProperty] private int _totalAuditLogs;
        [ObservableProperty] private int _totalBackups;
        [ObservableProperty] private string _backupStatus = "Ready";
        [ObservableProperty] private bool _isBackupInProgress;
        [ObservableProperty] private bool _isRestoreInProgress;
        [ObservableProperty] private bool _isCompressBackup;
        [ObservableProperty] private bool _autoBackupEnabled = true;
        [ObservableProperty] private int _backupRetentionDays = 30;
        [ObservableProperty] private string _systemStatusMessage = string.Empty;
        [ObservableProperty] private int _activeSessions;
        [ObservableProperty] private int _failedLogins24h;
        [ObservableProperty] private string _memoryUsage = "Loading...";
        [ObservableProperty] private string _cpuUsage = "Loading...";
        [ObservableProperty] private int _sessionTimeoutMinutes = 30;
        [ObservableProperty] private int _maxFailedLoginAttempts = 5;
        [ObservableProperty] private int _passwordExpiryDays = 90;
        [ObservableProperty] private bool _requireStrongPasswords = true;
        [ObservableProperty] private bool _enableAuditLogging = true;
        [ObservableProperty] private string _smtpServer = string.Empty;
        [ObservableProperty] private int _smtpPort = 587;
        [ObservableProperty] private string _smtpFromEmail = string.Empty;

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            private set => SetProperty(ref _isEditing, value);
        }

        [ObservableProperty] private bool _showStaffForm;

        [RelayCommand]
        private void CloseForm()
        {
            ClearForm();
        }

        [RelayCommand]
        private async Task LoadAll()
        {
            IsLoading = true;
            try
            {
                AllStaff.Clear();
                Administrators.Clear();
                HardwareTechnicians.Clear();
                ReportWriters.Clear();
                Physicians.Clear();
                Nurses.Clear();

                var all = await _staffService.GetAllStaffAsync();
                foreach (var s in all)
                {
                    AllStaff.Add(s);
                    switch (s.Role)
                    {
                        case StaffRole.Administrator: Administrators.Add(s); break;
                        case StaffRole.HardwareTechnician: HardwareTechnicians.Add(s); break;
                        case StaffRole.ReportWriter: ReportWriters.Add(s); break;
                        case StaffRole.Physician: Physicians.Add(s); break;
                        case StaffRole.Nurse: Nurses.Add(s); break;
                    }
                }

                ApplySearchFilter();

                TotalStaff = AllStaff.Count;
                ActiveAccounts = await _staffService.GetActiveAccountCountAsync();
                LockedAccounts = await _staffService.GetLockedAccountCountAsync();
                AdminCount = Administrators.Count;
                TechnicianCount = HardwareTechnicians.Count;
                ReportWriterCount = ReportWriters.Count;
                PhysicianCount = Physicians.Count;
                NurseCount = Nurses.Count;

                // Load license status & hospital info
                try
                {
                    var currentLicense = await _licenseService.GetCurrentLicenseAsync();
                    TotalLicenses = currentLicense != null ? 1 : 0;
                    ActiveLicenses = currentLicense?.IsActivated == true ? 1 : 0;
                    LicenseStatusText = await _licenseService.GetLicenseStatusTextAsync();
                    if (currentLicense != null)
                    {
                        HospitalName = currentLicense.HospitalName ?? string.Empty;
                        AdministratorName = currentLicense.AdministratorName ?? string.Empty;
                        InputLicenseNumber = currentLicense.LicenseNumber ?? string.Empty;
                    }
                }
                catch { LicenseStatusText = "No active license"; }

                // Load current user info
                if (_currentUserService.CurrentUser != null)
                {
                    CurrentUserName = _currentUserService.CurrentUser.FullName ?? _currentUserService.CurrentUser.Username ?? "Unknown";
                }

                // Load system info
                try
                {
                    var dbInfo = await _backupService.GetDatabaseInfoAsync();
                    DatabaseInfo = Path.GetFileName(dbInfo?.FilePath ?? "Unknown");
                    DatabaseSize = FormatFileSize(dbInfo?.FileSizeBytes ?? 0);
                    DiskSpace = FormatFileSize(await _backupService.GetAvailableDiskSpaceAsync(dbInfo?.FilePath ?? ""));
                    SystemVersion = "v1.0.0";
                    
                    // Calculate uptime
                    var process = System.Diagnostics.Process.GetCurrentProcess();
                    var uptime = DateTime.Now - process.StartTime;
                    SystemUptime = uptime.TotalDays > 1 
                        ? $"{(int)uptime.TotalDays}d {uptime.Hours}h {uptime.Minutes}m"
                        : $"{uptime.Hours}h {uptime.Minutes}m";

                    // Memory usage
                    var memoryMb = process.WorkingSet64 / (1024.0 * 1024.0);
                    MemoryUsage = $"{memoryMb:F1} MB";
                    
                    // CPU usage (approximate)
                    var cpuTime = process.TotalProcessorTime;
                    var elapsed = DateTime.Now - process.StartTime;
                    var cpuPercent = Math.Min(100, (cpuTime.TotalMilliseconds / elapsed.TotalMilliseconds) * 100);
                    CpuUsage = $"{cpuPercent:F0}%";

                    TotalAuditLogs = await _auditService.GetTotalAuditLogCountAsync();
                    var backupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
                    TotalBackups = (await _backupService.GetBackupHistoryAsync(backupDir)).Count;
                    
                    // Active sessions (from UserSession table via unit of work)
                    ActiveSessions = 1; // Current user
                    FailedLogins24h = 0; // Would need audit log query
                }
                catch (Exception ex)
                {
                    SystemStatusMessage = $"Error loading system info: {ex.Message}";
                }

                StatusMessage = $"Loaded {TotalStaff} staff • {TotalLicenses} license{(TotalLicenses != 1 ? "s" : "")} • System ready";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading staff: {ex.Message}";
            }
            finally { IsLoading = false; }
        }

        [RelayCommand]
        private void StartAddNew()
        {
            ShowStaffForm = true;
            SelectedStaff = null;
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
            SelectedRole = StaffRole.Staff;
            Department = string.Empty;
            JobTitle = string.Empty;
            Specialization = string.Empty;
            CanManageDevices = false;
            CanManageMaintenance = false;
            CanManageCalibration = false;
            CanViewReports = false;
            CanAccessAdminPanel = false;
            CanManageSpareParts = false;
            CanManageNetworkDevices = false;
            CanManageStaff = false;
            FormTitleText = "Add New Staff";
            FormSubtitle = "Create a new staff account";
            FormIcon = "➕";
            IsEditing = false;
        }

        [RelayCommand]
        private void StartEdit(StaffMember? staff)
        {
            if (staff == null)
            {
                _dialogService.ShowMessageAsync("Please select a staff member to edit.", "No Selection").ConfigureAwait(false);
                return;
            }
            ShowStaffForm = true;
            SelectedStaff = staff;
            FirstName = staff.FirstName;
            LastName = staff.LastName;
            Email = staff.Email;
            Phone = staff.Phone;
            Username = staff.Username;
            Password = string.Empty;
            SelectedRole = staff.Role;
            Department = staff.Department;
            JobTitle = staff.JobTitle;
            Specialization = staff.Specialization;
            CanManageDevices = staff.CanManageDevices;
            CanManageMaintenance = staff.CanManageMaintenance;
            CanManageCalibration = staff.CanManageCalibration;
            CanViewReports = staff.CanViewReports;
            CanAccessAdminPanel = staff.CanAccessAdminPanel;
            CanManageSpareParts = staff.CanManageSpareParts;
            CanManageNetworkDevices = staff.CanManageNetworkDevices;
            CanManageStaff = staff.CanManageStaff;
            FormTitleText = "Edit Staff Member";
            FormSubtitle = $"Editing: {staff.FullName} ({staff.EmployeeId})";
            FormIcon = "✏️";
            IsEditing = true;
        }

        [RelayCommand]
        private async Task SaveStaff()
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
            {
                await _dialogService.ShowMessageAsync("First name and last name are required.", "Validation Error");
                return;
            }

            if (string.IsNullOrWhiteSpace(Username))
            {
                await _dialogService.ShowMessageAsync("Username is required.", "Validation Error");
                return;
            }

            if (SelectedStaff == null && string.IsNullOrWhiteSpace(Password))
            {
                await _dialogService.ShowMessageAsync("Password is required for new staff members.", "Validation Error");
                return;
            }

            try
            {
                if (SelectedStaff != null)
                {
                    // Edit existing
                    SelectedStaff.FirstName = FirstName;
                    SelectedStaff.LastName = LastName;
                    SelectedStaff.Email = Email;
                    SelectedStaff.Phone = Phone;
                    SelectedStaff.Username = Username;
                    SelectedStaff.Role = SelectedRole;
                    SelectedStaff.Department = Department;
                    SelectedStaff.JobTitle = JobTitle;
                    SelectedStaff.Specialization = Specialization;
                    SelectedStaff.CanManageDevices = CanManageDevices;
                    SelectedStaff.CanManageMaintenance = CanManageMaintenance;
                    SelectedStaff.CanManageCalibration = CanManageCalibration;
                    SelectedStaff.CanViewReports = CanViewReports;
                    SelectedStaff.CanAccessAdminPanel = CanAccessAdminPanel;
                    SelectedStaff.CanManageSpareParts = CanManageSpareParts;
                    SelectedStaff.CanManageNetworkDevices = CanManageNetworkDevices;
                    SelectedStaff.CanManageStaff = CanManageStaff;
                    SelectedStaff.UpdatedAt = DateTime.Now;

                    if (!string.IsNullOrWhiteSpace(Password))
                    {
                        SelectedStaff.PasswordHash = HashPassword(Password);
                    }

                    await _staffService.UpdateStaffAsync(SelectedStaff);
                    StatusMessage = $"✅ Updated: {FirstName} {LastName}";
                }
                else
                {
                    // Create new
                    var staff = new StaffMember
                    {
                        EmployeeId = $"EMP-{DateTime.Now:yyyyMMddHHmmss}",
                        FirstName = FirstName,
                        LastName = LastName,
                        Email = Email,
                        Phone = Phone,
                        Username = Username,
                        PasswordHash = HashPassword(Password),
                        Role = SelectedRole,
                        Department = Department,
                        JobTitle = JobTitle,
                        Specialization = Specialization,
                        CanManageDevices = CanManageDevices,
                        CanManageMaintenance = CanManageMaintenance,
                        CanManageCalibration = CanManageCalibration,
                        CanViewReports = CanViewReports,
                        CanAccessAdminPanel = CanAccessAdminPanel,
                        CanManageSpareParts = CanManageSpareParts,
                        CanManageNetworkDevices = CanManageNetworkDevices,
                        CanManageStaff = CanManageStaff,
                        HireDate = DateTime.Now,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };

                    await _staffService.CreateStaffAsync(staff);
                    StatusMessage = $"✅ Created: {FirstName} {LastName} ({staff.EmployeeId})";
                }

                ClearForm();
                await LoadAllCommand.ExecuteAsync(null);
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Save error: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task DeleteStaff()
        {
            if (SelectedStaff == null)
            {
                await _dialogService.ShowMessageAsync("Select a staff member to delete.", "No Selection");
                return;
            }

            var confirmed = await _dialogService.ShowConfirmAsync(
                $"Are you sure you want to delete '{SelectedStaff.FullName}' ({SelectedStaff.Role})?\n\nThis action cannot be undone.", "Confirm Delete");

            if (confirmed)
            {
                await _staffService.DeleteStaffAsync(SelectedStaff.Id);
                StatusMessage = $"🗑️ Deleted: {SelectedStaff.FullName}";
                ClearForm();
                await LoadAllCommand.ExecuteAsync(null);
            }
        }

        [RelayCommand]
        private async Task ResetPassword()
        {
            if (SelectedStaff == null)
            {
                await _dialogService.ShowMessageAsync("Select a staff member.", "No Selection");
                return;
            }

            // Generate random secure password
            var newPass = GenerateSecurePassword();
            await _staffService.ResetPasswordAsync(SelectedStaff.Id, HashPassword(newPass));
            
            await _dialogService.ShowMessageAsync(
                $"Password reset for {SelectedStaff.Username}\n\n" +
                $"Employee: {SelectedStaff.FullName}\n" +
                $"Temporary Password: {newPass}\n\n" +
                "Please communicate this securely to the user.", "Password Reset");
            
            StatusMessage = $"🔑 Password reset for {SelectedStaff.Username}";
        }

        [RelayCommand]
        private async Task ToggleLock()
        {
            if (SelectedStaff == null)
            {
                await _dialogService.ShowMessageAsync("Select a staff member.", "No Selection");
                return;
            }

            bool willLock = !SelectedStaff.IsLocked;
            var confirmed = await _dialogService.ShowConfirmAsync(
                $"{(willLock ? "Lock" : "Unlock")} account for {SelectedStaff.Username}?", 
                $"Confirm {(willLock ? "Lock" : "Unlock")}");

            if (confirmed)
            {
                await _staffService.ToggleAccountLockAsync(SelectedStaff.Id, willLock);
                StatusMessage = $"🔒 Account {(willLock ? "locked" : "unlocked")}: {SelectedStaff.Username}";
                await LoadAllCommand.ExecuteAsync(null);
            }
        }

        [RelayCommand]
        private async Task GenerateLicense()
        {
            try
            {
                var licenseType = SelectedLicenseTypeIndex switch
                {
                    0 => LicenseType.ThreeMonths,
                    1 => LicenseType.OneYear,
                    _ => LicenseType.Lifetime
                };

                GeneratedLicenseKey = _licenseService.GenerateLicenseKey(licenseType);
                StatusMessage = $"Generated {licenseType} license key";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                await _dialogService.ShowMessageAsync($"Error generating license: {ex.Message}", "Error");
            }
        }

        [RelayCommand]
        private async Task CopyLicenseToClipboard()
        {
            if (string.IsNullOrWhiteSpace(GeneratedLicenseKey))
            {
                await _dialogService.ShowMessageAsync("Generate a license key first.", "No License");
                return;
            }
            Clipboard.SetText(GeneratedLicenseKey);
            StatusMessage = $"📋 License key copied to clipboard: {GeneratedLicenseKey}";
        }

        // Update System Commands
        [RelayCommand]
        private async Task GenerateLicenseFile()
        {
            if (string.IsNullOrWhiteSpace(HospitalName) || string.IsNullOrWhiteSpace(AdministratorName))
            {
                UpdateStatus = "⚠️ Please fill in hospital name and administrator name first.";
                return;
            }

            var licenseType = SelectedLicenseTypeIndex switch
            {
                0 => LicenseType.ThreeMonths,
                1 => LicenseType.OneYear,
                _ => LicenseType.Lifetime
            };

            // Open save file dialog
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "License Files (*.license)|*.license|All Files (*.*)|*.*",
                DefaultExt = ".license",
                FileName = $"MomenMedmSys_{licenseType}_{DateTime.Now:yyyyMMdd}.license",
                Title = "Save License File"
            };

            if (dialog.ShowDialog() == true)
            {
                UpdateStatus = "💾 Generating license file...";
                StatusMessage = "⏳ Generating license file...";

                var result = await _licenseService.GenerateLicenseFileAsync(
                    dialog.FileName, licenseType, HospitalName, AdministratorName, InputLicenseNumber);

                if (result.Success)
                {
                    UpdateStatus = $"✅ {result.Message}";
                    StatusMessage = $"🔑 License file generated: {dialog.FileName}";
                    GeneratedLicenseKey = result.License?.LicenseKey ?? string.Empty;
                }
                else
                {
                    UpdateStatus = $"❌ {result.Message}";
                    StatusMessage = $"❌ Failed to generate license file";
                }
            }
        }

        [RelayCommand]
        private async Task UpdateSystem()
        {
            if (string.IsNullOrWhiteSpace(HospitalName) || string.IsNullOrWhiteSpace(AdministratorName))
            {
                UpdateStatus = "⚠️ Please fill in hospital name and administrator name first.";
                return;
            }

            var licenseType = SelectedLicenseTypeIndex switch
            {
                0 => LicenseType.ThreeMonths,
                1 => LicenseType.OneYear,
                _ => LicenseType.Lifetime
            };

            var confirmed = await _dialogService.ShowConfirmAsync(
                $"This will generate a new system package for:\n\n" +
                $"🏥 Hospital: {HospitalName}\n" +
                $"👤 Administrator: {AdministratorName}\n" +
                $"🔑 License Type: {licenseType}\n" +
                $"📄 License #: {InputLicenseNumber}\n\n" +
                $"The package will include a license file for the user to activate the system.\n\n" +
                $"Continue?",
                "Confirm System Update");

            if (!confirmed) return;

            IsUpdateInProgress = true;
            UpdateStatus = "🔄 Updating system package...";
            StatusMessage = "⏳ Generating system update package...";

            try
            {
                // Open folder browser to select output directory
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "All Files (*.*)|*.*",
                    DefaultExt = "",
                    FileName = $"MomenMedmSys_{HospitalName.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}",
                    Title = "Save System Package"
                };

                if (dialog.ShowDialog() == true)
                {
                    var result = await _licenseService.UpdateSystemExecutableAsync(
                        dialog.FileName, licenseType, HospitalName, AdministratorName, InputLicenseNumber);

                    if (result.Success)
                    {
                        UpdateStatus = $"✅ {result.Message}";
                        StatusMessage = $"🎉 System package created successfully!";
                        GeneratedLicenseKey = result.License?.LicenseKey ?? string.Empty;

                        // Reload license info
                        await LoadAllCommand.ExecuteAsync(null);

                        await _dialogService.ShowMessageAsync(
                            $"System package generated successfully!\n\n" +
                            $"📁 Package: {dialog.FileName}\n" +
                            $"🔑 License Key: {GeneratedLicenseKey}\n\n" +
                            $"Send these files to the hospital for installation.",
                            "System Update Complete");
                    }
                    else
                    {
                        UpdateStatus = $"❌ {result.Message}";
                        StatusMessage = "❌ Failed to update system package";
                    }
                }
                else
                {
                    UpdateStatus = "⏹️ Operation cancelled by user.";
                    StatusMessage = "System update cancelled";
                }
            }
            catch (Exception ex)
            {
                UpdateStatus = $"❌ Error: {ex.Message}";
                StatusMessage = $"❌ Update failed: {ex.Message}";
            }
            finally { IsUpdateInProgress = false; }
        }

        // Distribution Package Generation Command
        [RelayCommand]
        private async Task GenerateDistributionPackage()
        {
            if (string.IsNullOrWhiteSpace(HospitalName) || string.IsNullOrWhiteSpace(AdministratorName))
            {
                DistributionStatus = "⚠️ Please fill in hospital name and administrator name first.";
                return;
            }

            var licenseType = SelectedLicenseTypeIndex switch
            {
                0 => LicenseType.ThreeMonths,
                1 => LicenseType.OneYear,
                _ => LicenseType.Lifetime
            };

            var confirmed = await _dialogService.ShowConfirmAsync(
                $"This will generate a complete distribution package for:\n\n" +
                $"🏥 Hospital: {HospitalName}\n" +
                $"👤 Administrator: {AdministratorName}\n" +
                $"🔑 License Type: {licenseType}\n" +
                $"📄 License #: {InputLicenseNumber}\n\n" +
                $"The package will include:\n" +
                $"  • Application executable\n" +
                $"  • Database file\n" +
                $"  • License file\n" +
                $"  • README with installation instructions\n\n" +
                $"Continue?",
                "Confirm Distribution Package Generation");

            if (!confirmed) return;

            IsDistributionInProgress = true;
            DistributionStatus = "🔄 Generating distribution package...";
            StatusMessage = "⏳ Creating distribution package...";

            try
            {
                // Use SaveFileDialog to select output location (creates a folder)
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Folder Selection (*.folder)|*.folder",
                    DefaultExt = "folder",
                    FileName = $"MomenMedmSys_{HospitalName.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}",
                    Title = "Select Location to Save Distribution Package (a folder will be created)"
                };

                if (saveDialog.ShowDialog() != true)
                {
                    DistributionStatus = "⏹️ Operation cancelled by user.";
                    StatusMessage = "Distribution package generation cancelled";
                    IsDistributionInProgress = false;
                    return;
                }

                // Get the directory from the file path
                var packagePath = System.IO.Path.GetDirectoryName(saveDialog.FileName);
                var packageName = System.IO.Path.GetFileNameWithoutExtension(saveDialog.FileName);
                
                if (string.IsNullOrEmpty(packagePath) || string.IsNullOrEmpty(packageName))
                {
                    DistributionStatus = "❌ Invalid output path selected.";
                    StatusMessage = "Invalid path";
                    IsDistributionInProgress = false;
                    return;
                }

                // Create full path with package name
                packagePath = System.IO.Path.Combine(packagePath, packageName);

                // Create package directory
                if (Directory.Exists(packagePath))
                {
                    Directory.Delete(packagePath, true);
                }
                Directory.CreateDirectory(packagePath);

                // Create subdirectories
                var appDir = Path.Combine(packagePath, "Application");
                var licenseDir = Path.Combine(packagePath, "License");
                var docsDir = Path.Combine(packagePath, "Documentation");
                Directory.CreateDirectory(appDir);
                Directory.CreateDirectory(licenseDir);
                Directory.CreateDirectory(docsDir);

                DistributionStatus = "📦 Copying application files...";

                // Copy application files
                var appRoot = AppDomain.CurrentDomain.BaseDirectory;
                var filesToCopy = new List<string>();

                // Get all files except database and logs
                var allFiles = Directory.GetFiles(appRoot, "*.*", SearchOption.AllDirectories);
                var excludePatterns = new[] { ".db", ".log", ".license", ".json" };

                int copiedFiles = 0;
                int totalFiles = 0;

                foreach (var file in allFiles)
                {
                    var relativePath = Path.GetRelativePath(appRoot, file);

                    // Skip certain directories
                    if (relativePath.StartsWith("Backups") ||
                        relativePath.StartsWith("Logs") ||
                        relativePath.StartsWith("Temp"))
                        continue;

                    var ext = Path.GetExtension(file).ToLower();

                    // Skip database, logs, and existing license files
                    if (excludePatterns.Contains(ext))
                        continue;

                    var destFile = Path.Combine(appDir, relativePath);
                    var destDir = Path.GetDirectoryName(destFile);

                    if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                    {
                        Directory.CreateDirectory(destDir);
                    }

                    try
                    {
                        File.Copy(file, destFile, true);
                        copiedFiles++;
                    }
                    catch
                    {
                        // Skip files that are in use or can't be copied
                    }
                    totalFiles++;
                }

                DistributionStatus = "📄 Generating license file...";

                // Generate license file
                var licenseFileName = $"{packageName}.license";
                var licenseFilePath = Path.Combine(licenseDir, licenseFileName);

                var licenseResult = await _licenseService.GenerateLicenseFileAsync(
                    licenseFilePath, licenseType, HospitalName, AdministratorName, InputLicenseNumber);

                if (!licenseResult.Success)
                {
                    DistributionStatus = $"❌ Failed to generate license: {licenseResult.Message}";
                    StatusMessage = "❌ License generation failed";
                    IsDistributionInProgress = false;
                    return;
                }

                DistributionStatus = "📋 Generating documentation...";

                // Generate README file
                var readmePath = Path.Combine(docsDir, "README.txt");
                var readmeContent = GenerateReadmeContent(packageName, licenseType, licenseResult.License?.LicenseKey ?? "");
                await File.WriteAllTextAsync(readmePath, readmeContent);

                // Generate installation guide
                var installGuidePath = Path.Combine(docsDir, "InstallationGuide.txt");
                var installGuideContent = GenerateInstallationGuide();
                await File.WriteAllTextAsync(installGuidePath, installGuideContent);

                // Copy database template (empty or with seed data)
                var dbPath = Path.Combine(appRoot, "medmsys.db");
                if (File.Exists(dbPath))
                {
                    var dbDestPath = Path.Combine(appDir, "medmsys.db");
                    File.Copy(dbPath, dbDestPath, true);
                }

                // Create a batch file to launch the application
                var exeName = "MomenMedmSys.WPF.exe";
                var exePath = Path.Combine(appDir, exeName);
                var sourceExePath = Path.Combine(appRoot, exeName);
                
                // If exe doesn't exist in app root, try to find it
                if (!File.Exists(sourceExePath))
                {
                    var possiblePaths = new[] {
                        Path.Combine(appRoot, "bin", "Release", "net8.0-windows", exeName),
                        Path.Combine(appRoot, "bin", "Debug", "net8.0-windows", exeName),
                        Path.Combine(Directory.GetParent(appRoot)?.FullName ?? "", exeName),
                        Path.Combine(Directory.GetParent(Directory.GetParent(appRoot)?.FullName ?? "")?.FullName ?? "", "bin", "Release", "net8.0-windows", exeName)
                    };
                    foreach (var p in possiblePaths)
                    {
                        if (File.Exists(p))
                        {
                            sourceExePath = p;
                            break;
                        }
                    }
                }
                
                // Copy exe explicitly
                if (File.Exists(sourceExePath) && !File.Exists(exePath))
                {
                    File.Copy(sourceExePath, exePath, true);
                }
                
                if (File.Exists(exePath))
                {
                    var launchBat = Path.Combine(packagePath, "START.bat");
                    var batContent = $@"@echo off
echo Starting MomenMedmSys - {HospitalName}
echo.
cd Application
start """""""" ""{exeName}""
";
                    await File.WriteAllTextAsync(launchBat, batContent);
                }

                // Create package info file
                var packageInfoPath = Path.Combine(packagePath, "PackageInfo.txt");
                var packageInfo = $@"MomenMedmSys Distribution Package
=====================================
Package Name: {packageName}
Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
Hospital: {HospitalName}
Administrator: {AdministratorName}
License Number: {InputLicenseNumber}
License Type: {licenseType}
License Key: {licenseResult.License?.LicenseKey ?? "N/A"}

Contents:
  - Application: Complete application files
  - Database: medmsys.db (SQLite database)
  - License: {licenseFileName}
  - Documentation: README and Installation Guide

Instructions:
  1. Read the README.txt in Documentation folder
  2. Follow the InstallationGuide.txt
  3. Copy the entire Application folder to target machine
  4. Place the license file in the Application folder
  5. Run MomenMedmSys.WPF.exe

Support: MOHS2N@YAHOO.COM | Tel: +249 124 349 024
© 2026 Momen Systems Company. All Rights Reserved.
";
                await File.WriteAllTextAsync(packageInfoPath, packageInfo);

                DistributionStatus = $"✅ Package generated: {packageName}";
                StatusMessage = $"🎉 Distribution package created successfully!";
                GeneratedLicenseKey = licenseResult.License?.LicenseKey ?? string.Empty;

                // Create zip file
                var zipPath = $"{packagePath}.zip";
                if (File.Exists(zipPath)) File.Delete(zipPath);
                
                DistributionStatus = "Creating ZIP archive...";
                System.IO.Compression.ZipFile.CreateFromDirectory(packagePath, zipPath);
                
                // Clean up folder, keep only zip
                Directory.Delete(packagePath, true);
                
                var zipFileName = System.IO.Path.GetFileName(zipPath);
                var zipSizeMB = new System.IO.FileInfo(zipPath).Length / (1024.0 * 1024.0);

                // Reload license info
                await LoadAllCommand.ExecuteAsync(null);

                await _dialogService.ShowMessageAsync(
                    $"Distribution package created successfully!\n\n" +
                    $"📦 ZIP: {zipFileName}\n" +
                    $"📏 Size: {zipSizeMB:F1} MB\n" +
                    $"🔑 License Key: {GeneratedLicenseKey}\n\n" +
                    $"Send the ZIP file to the hospital.",
                    "Distribution Package Complete");
            }
            catch (Exception ex)
            {
                DistributionStatus = $"❌ Error: {ex.Message}";
                StatusMessage = $"❌ Distribution failed: {ex.Message}";
            }
            finally { IsDistributionInProgress = false; }
        }

        private string GenerateReadmeContent(string packageName, LicenseType licenseType, string licenseKey)
        {
            return $@"=====================================================
  MOMENMEDMSYS - MEDICAL EQUIPMENT MANAGEMENT SYSTEM
  Distribution Package - README
=====================================================

Package: {packageName}
Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}

-----------------------------------------------------
LICENSE INFORMATION
-----------------------------------------------------
License Type: {licenseType}
License Key: {licenseKey}

License Duration:
{(licenseType == LicenseType.ThreeMonths ? "  • Valid for 3 months (90 days) from activation" : 
  licenseType == LicenseType.OneYear ? "  • Valid for 1 year (365 days) from activation" : 
  "  • Lifetime license - never expires")}

-----------------------------------------------------
SYSTEM REQUIREMENTS
-----------------------------------------------------
  • Windows 10 or Windows 11 (64-bit)
  • .NET 8.0 Runtime (included in application folder)
  • Minimum 4GB RAM
  • Minimum 2GB free disk space
  • Administrator privileges for installation

-----------------------------------------------------
QUICK START
-----------------------------------------------------
1. Extract the entire package to a folder
2. Copy all files from Application/ to installation folder
3. Place the .license file in the same folder as the .exe
4. Double-click MomenMedmSys.WPF.exe to start
5. On first run, the system will activate using the license file

-----------------------------------------------------
INSTALLATION
-----------------------------------------------------
See InstallationGuide.txt for detailed instructions.

-----------------------------------------------------
SUPPORT & CONTACT
-----------------------------------------------------
Email: MOHS2N@YAHOO.COM
Phone: +249 124 349 024

-----------------------------------------------------
LICENSE & WARRANTY
-----------------------------------------------------
© 2026 Momen Systems Company. All Rights Reserved.
This software is licensed for use by the named hospital only.
Unauthorized distribution is prohibited.

=====================================================
";
        }

        private string GenerateInstallationGuide()
        {
            return @"=====================================================
  INSTALLATION GUIDE - MomenMedmSys
  Medical Equipment Management System
=====================================================

PREREQUISITES
-----------------------------------------------------
• Windows 10/11 (64-bit)
• Administrator access to the computer
• 2GB free disk space minimum

INSTALLATION STEPS
-----------------------------------------------------

STEP 1: Prepare Installation Directory
  1. Create a new folder (e.g., C:\MomenMedmSys)
  2. This will be the installation directory

STEP 2: Copy Application Files
  1. Open the Application folder from this package
  2. Select ALL files and folders
  3. Copy them to your installation directory
  4. Ensure the following files are present:
     • MomenMedmSys.WPF.exe (main application)
     • medmsys.db (SQLite database)
     • All .dll files (application libraries)

STEP 3: Install License File
  1. Open the License folder from this package
  2. Copy the .license file to the installation directory
  3. The license file MUST be in the same folder as the .exe
  4. Example: C:\MomenMedmSys\MomenMedmSys_Hospital_20260413.license

STEP 4: First Run & Activation
  1. Double-click MomenMedmSys.WPF.exe
  2. The system will automatically detect the license file
  3. Login with default credentials:
     Username: admin
     Password: Admin@123
  4. Change the default password immediately after first login

STEP 5: Verify Installation
  1. Open Admin Control Panel (9th tab in navigation)
  2. Check the Licenses tab
  3. Verify the license status shows as ""Active""
  4. Confirm hospital name is displayed correctly

TROUBLESHOOTING
-----------------------------------------------------

Q: Application won't start
A: Ensure .NET 8.0 Runtime is installed
   Download from: https://dotnet.microsoft.com/download

Q: License not detected
A: Verify the .license file is in the same folder as the .exe
   Check that the license file name matches the pattern

Q: Database errors
A: Ensure the application has read/write access to the folder
   Check that medmsys.db is present and not corrupted

Q: Permission denied
A: Run the application as Administrator (right-click -> Run as Administrator)
   Check Windows folder permissions

UNINSTALLATION
-----------------------------------------------------
1. Close the application
2. Delete the installation folder
3. Optionally delete database backup files
4. No registry entries are created

BACKUP & RECOVERY
-----------------------------------------------------
• Regular backups are created automatically in the Backups folder
• Manual backups can be created from Admin Control Panel > System tab
• To restore: use the Restore function in Admin Control Panel

SECURITY NOTES
-----------------------------------------------------
• Change default password immediately after installation
• Do not share the license file with unauthorized users
• Keep backup files in a secure location
• Regularly review audit logs for unauthorized access

SUPPORT
-----------------------------------------------------
For technical support or questions:
Email: MOHS2N@YAHOO.COM
Phone: +249 124 349 024

=====================================================
© 2026 Momen Systems Company. All Rights Reserved.
=====================================================
";
        }

        // Hospital Information Commands
        [RelayCommand]
        private async Task SaveHospitalInfo()
        {
            if (string.IsNullOrWhiteSpace(HospitalName) || string.IsNullOrWhiteSpace(AdministratorName))
            {
                HospitalInfoStatus = "⚠️ Hospital name and administrator name are required.";
                return;
            }

            HospitalInfoStatus = "💾 Saving hospital information...";
            var result = await _licenseService.UpdateHospitalInfoAsync(HospitalName, AdministratorName, InputLicenseNumber);

            if (result.Success)
            {
                HospitalInfoStatus = $"✅ {result.Message}";
                StatusMessage = $"🏥 Hospital info saved: {HospitalName} — {AdministratorName}";
            }
            else
            {
                HospitalInfoStatus = $"❌ {result.Message}";
                StatusMessage = $"⚠️ Failed to save hospital info: {result.Message}";
            }
        }

        // System Management Commands
        [RelayCommand]
        private async Task CreateBackup()
        {
            if (IsBackupInProgress) return;
            
            IsBackupInProgress = true;
            BackupStatus = "Creating backup...";
            StatusMessage = "⏳ Creating database backup...";

            var backupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");

            try
            {
                var backup = await _backupService.CreateTimestampedBackupAsync(backupDir);
                DatabaseSize = FormatFileSize(backup.FileSizeBytes);
                TotalBackups = (await _backupService.GetBackupHistoryAsync(backupDir)).Count;

                if (IsCompressBackup)
                {
                    BackupStatus = "Compressing backup...";
                    backup = await _backupService.CompressBackupAsync(backup.FilePath);
                }

                BackupStatus = $"✅ Backup created: {backup.FileName}";
                StatusMessage = $"💾 Backup created successfully: {backup.FileName}";
            }
            catch (Exception ex)
            {
                BackupStatus = $"❌ Backup failed: {ex.Message}";
                StatusMessage = $"❌ Backup failed: {ex.Message}";
            }
            finally { IsBackupInProgress = false; }
        }

        [RelayCommand]
        private async Task RestoreBackup()
        {
            if (IsRestoreInProgress) return;

            var confirmed = await _dialogService.ShowConfirmAsync(
                "⚠️ WARNING: This will replace your current database with the backup.\n\n" +
                "Are you sure you want to restore? All current data will be lost.",
                "Confirm Database Restore");

            if (!confirmed) return;

            IsRestoreInProgress = true;
            BackupStatus = "Restoring backup...";
            StatusMessage = "⏳ Restoring database from backup...";

            var backupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");

            try
            {
                var dbInfo = await _backupService.GetDatabaseInfoAsync();
                var dbPath = dbInfo?.FilePath;
                
                if (string.IsNullOrEmpty(dbPath))
                {
                    throw new Exception("Could not determine database path");
                }

                // Get latest backup
                var backups = await _backupService.GetBackupHistoryAsync(backupDir);
                var latestBackup = backups.OrderByDescending(b => b.CreatedAt).FirstOrDefault();
                
                if (latestBackup == null)
                {
                    throw new Exception("No backups available to restore");
                }

                await _backupService.RestoreBackupAsync(latestBackup.FilePath, dbPath);
                
                BackupStatus = $"✅ Restored: {latestBackup.FileName}";
                StatusMessage = $"🔄 Database restored from: {latestBackup.FileName}";

                // Reload all data
                await LoadAllCommand.ExecuteAsync(null);
            }
            catch (Exception ex)
            {
                BackupStatus = $"❌ Restore failed: {ex.Message}";
                StatusMessage = $"❌ Restore failed: {ex.Message}";
            }
            finally { IsRestoreInProgress = false; }
        }

        [RelayCommand]
        private async Task CleanupOldBackups()
        {
            var backupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");

            var confirmed = await _dialogService.ShowConfirmAsync(
                $"Delete backups older than {BackupRetentionDays} days?\n\n" +
                $"This will permanently remove old backup files.",
                "Confirm Cleanup");

            if (!confirmed) return;

            try
            {
                var deleted = await _backupService.CleanupOldBackupsAsync(backupDir, BackupRetentionDays);
                TotalBackups = (await _backupService.GetBackupHistoryAsync(backupDir)).Count;
                
                BackupStatus = $"🗑️ Deleted {deleted} old backup{(deleted != 1 ? "s" : "")}";
                StatusMessage = $"🗑️ Cleaned up {deleted} old backups";
            }
            catch (Exception ex)
            {
                BackupStatus = $"❌ Cleanup failed: {ex.Message}";
                StatusMessage = $"❌ Cleanup failed: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task ValidateBackups()
        {
            var backupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");

            BackupStatus = "Validating backups...";
            StatusMessage = "🔍 Validating backup integrity...";

            try
            {
                var backups = await _backupService.GetBackupHistoryAsync(backupDir);
                int valid = 0, invalid = 0;

                foreach (var backup in backups)
                {
                    var result = await _backupService.ValidateBackupAsync(backup.FilePath);
                    if (result.IsValid) valid++;
                    else invalid++;
                }

                BackupStatus = $"✅ {valid} valid, ❌ {invalid} invalid out of {backups.Count} backups";
                StatusMessage = $"🔍 Validation complete: {valid} valid, {invalid} invalid";
            }
            catch (Exception ex)
            {
                BackupStatus = $"❌ Validation failed: {ex.Message}";
                StatusMessage = $"❌ Validation failed: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task OpenBackupFolder()
        {
            var backupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = backupDir,
                    UseShellExecute = true
                });
                StatusMessage = $"📂 Opened backup folder: {backupDir}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Could not open folder: {ex.Message}";
            }
        }

        // System Management Commands
        [RelayCommand]
        private async Task ClearCache()
        {
            StatusMessage = "🧹 Clearing application cache...";
            try
            {
                // Clear temp files
                var tempDir = Path.GetTempPath();
                var appTempFiles = Directory.GetFiles(tempDir, "MomenMedmSys_*");
                int deleted = 0;
                foreach (var file in appTempFiles)
                {
                    try { File.Delete(file); deleted++; } catch { }
                }

                // Clear recent files
                var recentDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recent");
                if (Directory.Exists(recentDir))
                {
                    foreach (var file in Directory.GetFiles(recentDir))
                    {
                        try { File.Delete(file); deleted++; } catch { }
                    }
                }

                // Force GC
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                var process = System.Diagnostics.Process.GetCurrentProcess();
                MemoryUsage = $"{process.WorkingSet64 / (1024.0 * 1024.0):F1} MB";
                
                StatusMessage = $"✅ Cache cleared: {deleted} temp files removed";
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Failed to clear cache: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task OptimizeDatabase()
        {
            StatusMessage = "📊 Optimizing database...";
            BackupStatus = "Running VACUUM and ANALYZE...";
            try
            {
                // Execute SQLite optimization commands
                using var connection = new Microsoft.Data.Sqlite.SqliteConnection(
                    $"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MomenMedmSys.db")}");
                await connection.OpenAsync();
                
                // VACUUM rebuilds the database file
                using var vacuumCmd = connection.CreateCommand();
                vacuumCmd.CommandText = "VACUUM;";
                await vacuumCmd.ExecuteNonQueryAsync();
                
                // ANALYZE updates statistics
                using var analyzeCmd = connection.CreateCommand();
                analyzeCmd.CommandText = "ANALYZE;";
                await analyzeCmd.ExecuteNonQueryAsync();
                
                // Update database size
                var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MomenMedmSys.db");
                var fileInfo = new FileInfo(dbPath);
                DatabaseSize = FormatFileSize(fileInfo.Length);
                
                StatusMessage = "✅ Database optimized successfully";
                BackupStatus = "✅ Optimization complete";
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Database optimization failed: {ex.Message}";
                BackupStatus = $"❌ Failed: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task ExportAuditLogs()
        {
            StatusMessage = "📝 Exporting audit logs...";
            try
            {
                var exportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                    $"AuditLog_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
                await _auditService.ExportAuditLogsAsync(exportPath);
                StatusMessage = $"✅ Audit logs exported: {exportPath}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Export failed: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task ForceLogoutAll()
        {
            var confirmed = await _dialogService.ShowConfirmAsync(
                "⚠️ This will force logout ALL active users except yourself.\n\n" +
                "All unsaved work will be lost.\n\n" +
                "Are you sure you want to continue?",
                "Confirm Force Logout");

            if (!confirmed) return;

            StatusMessage = "🚪 Forcing logout of all users...";
            try
            {
                // Get current user's ID to exclude
                var currentUserId = _currentUserService.UserId;
                
                // Close all other active sessions
                var allSessions = await _unitOfWork.UserSessions.FindAsync(s => s.LogoutTime == null);
                int loggedOut = 0;
                
                foreach (var session in allSessions)
                {
                    // Skip current user's active session
                    if (currentUserId.HasValue && session.UserId == currentUserId.Value && session.IsActive)
                        continue;
                        
                    session.LogoutTime = DateTime.Now;
                    session.IsActive = false;
                    _unitOfWork.UserSessions.Update(session);
                    loggedOut++;
                }
                
                await _unitOfWork.SaveChangesAsync();
                
                // Update session count
                ActiveSessions = 1; // Only current user
                
                StatusMessage = $"✅ {loggedOut} user(s) logged out successfully";
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Failed to logout users: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task SaveSystemSettings()
        {
            StatusMessage = "💾 Saving system settings...";
            try
            {
                // Save settings to JSON config file
                var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "system-settings.json");
                var settings = new
                {
                    SessionTimeoutMinutes = SessionTimeoutMinutes,
                    MaxFailedLoginAttempts = MaxFailedLoginAttempts,
                    PasswordExpiryDays = PasswordExpiryDays,
                    RequireStrongPasswords = RequireStrongPasswords,
                    EnableAuditLogging = EnableAuditLogging,
                    AutoBackupEnabled = AutoBackupEnabled,
                    BackupRetentionDays = BackupRetentionDays,
                    SmtpServer = SmtpServer,
                    SmtpPort = SmtpPort,
                    SmtpFromEmail = SmtpFromEmail,
                    LastUpdated = DateTime.Now
                };
                
                var json = System.Text.Json.JsonSerializer.Serialize(settings, new System.Text.Json.JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                
                File.WriteAllText(configPath, json);
                
                StatusMessage = "✅ System settings saved successfully";
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Failed to save settings: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task LoadSystemSettings()
        {
            StatusMessage = "📂 Loading system settings...";
            try
            {
                var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "system-settings.json");
                if (File.Exists(configPath))
                {
                    var json = await File.ReadAllTextAsync(configPath);
                    var settings = System.Text.Json.JsonSerializer.Deserialize<dynamic>(json);
                    
                    if (settings != null)
                    {
                        SessionTimeoutMinutes = settings.SessionTimeoutMinutes?.GetValue<int>() ?? 30;
                        MaxFailedLoginAttempts = settings.MaxFailedLoginAttempts?.GetValue<int>() ?? 5;
                        PasswordExpiryDays = settings.PasswordExpiryDays?.GetValue<int>() ?? 90;
                        RequireStrongPasswords = settings.RequireStrongPasswords?.GetValue<bool>() ?? true;
                        EnableAuditLogging = settings.EnableAuditLogging?.GetValue<bool>() ?? true;
                        AutoBackupEnabled = settings.AutoBackupEnabled?.GetValue<bool>() ?? true;
                        BackupRetentionDays = settings.BackupRetentionDays?.GetValue<int>() ?? 30;
                        SmtpServer = settings.SmtpServer?.GetValue<string>() ?? string.Empty;
                        SmtpPort = settings.SmtpPort?.GetValue<int>() ?? 587;
                        SmtpFromEmail = settings.SmtpFromEmail?.GetValue<string>() ?? string.Empty;
                    }
                }
                StatusMessage = "✅ System settings loaded";
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Failed to load settings: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task TestSmtpConnection()
        {
            if (string.IsNullOrWhiteSpace(SmtpServer))
            {
                await _dialogService.ShowMessageAsync("Please enter SMTP server details first.", "Missing Configuration");
                return;
            }
            
            StatusMessage = "📧 Testing SMTP connection...";
            try
            {
                // Attempt TCP connection to SMTP server
                using var tcpClient = new System.Net.Sockets.TcpClient();
                var connectTask = tcpClient.ConnectAsync(SmtpServer, SmtpPort);
                
                if (await Task.WhenAny(connectTask, Task.Delay(5000)) == connectTask)
                {
                    await connectTask; // Wait for completion
                    tcpClient.Close();
                    StatusMessage = $"✅ SMTP connection successful: {SmtpServer}:{SmtpPort}";
                }
                else
                {
                    StatusMessage = $"❌ SMTP connection timed out (5s): {SmtpServer}:{SmtpPort}";
                }
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                StatusMessage = $"❌ SMTP connection failed: {ex.Message}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ SMTP test failed: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task GenerateSystemReport()
        {
            StatusMessage = "📊 Generating system report...";
            try
            {
                var reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                    $"SystemReport_{DateTime.Now:yyyyMMdd}.txt");
                
                using var writer = new StreamWriter(reportPath);
                
                await writer.WriteLineAsync("╔══════════════════════════════════════════╗");
                await writer.WriteLineAsync("║    MOMEN MEDICAL MANAGEMENT SYSTEM       ║");
                await writer.WriteLineAsync("║         SYSTEM REPORT                    ║");
                await writer.WriteLineAsync($"║    Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                await writer.WriteLineAsync("╚══════════════════════════════════════════╝");
                await writer.WriteLineAsync();
                
                await writer.WriteLineAsync("SYSTEM INFORMATION");
                await writer.WriteLineAsync($"  Version: {SystemVersion}");
                await writer.WriteLineAsync($"  Database: {DatabaseInfo}");
                await writer.WriteLineAsync($"  DB Size: {DatabaseSize}");
                await writer.WriteLineAsync($"  Disk Space: {DiskSpace}");
                await writer.WriteLineAsync($"  Uptime: {SystemUptime}");
                await writer.WriteLineAsync();
                
                await writer.WriteLineAsync("PERFORMANCE METRICS");
                await writer.WriteLineAsync($"  Memory Usage: {MemoryUsage}");
                await writer.WriteLineAsync($"  CPU Usage: {CpuUsage}");
                await writer.WriteLineAsync($"  Active Sessions: {ActiveSessions}");
                await writer.WriteLineAsync($"  Failed Logins (24h): {FailedLogins24h}");
                await writer.WriteLineAsync();
                
                await writer.WriteLineAsync("STAFF STATISTICS");
                await writer.WriteLineAsync($"  Total Staff: {TotalStaff}");
                await writer.WriteLineAsync($"  Active Accounts: {ActiveAccounts}");
                await writer.WriteLineAsync($"  Locked Accounts: {LockedAccounts}");
                await writer.WriteLineAsync($"  Administrators: {AdminCount}");
                await writer.WriteLineAsync($"  Technicians: {TechnicianCount}");
                await writer.WriteLineAsync();
                
                await writer.WriteLineAsync("LICENSE STATUS");
                await writer.WriteLineAsync($"  Total Licenses: {TotalLicenses}");
                await writer.WriteLineAsync($"  Active Licenses: {ActiveLicenses}");
                await writer.WriteLineAsync($"  Status: {LicenseStatusText}");
                await writer.WriteLineAsync();
                
                await writer.WriteLineAsync("BACKUP STATUS");
                await writer.WriteLineAsync($"  Total Backups: {TotalBackups}");
                await writer.WriteLineAsync($"  Auto Backup: {(AutoBackupEnabled ? "Enabled" : "Disabled")}");
                await writer.WriteLineAsync($"  Retention: {BackupRetentionDays} days");
                await writer.WriteLineAsync();
                
                await writer.WriteLineAsync("SECURITY SETTINGS");
                await writer.WriteLineAsync($"  Session Timeout: {SessionTimeoutMinutes} min");
                await writer.WriteLineAsync($"  Max Failed Logins: {MaxFailedLoginAttempts}");
                await writer.WriteLineAsync($"  Password Expiry: {PasswordExpiryDays} days");
                await writer.WriteLineAsync($"  Strong Passwords: {(RequireStrongPasswords ? "Required" : "Not Required")}");
                await writer.WriteLineAsync($"  Audit Logging: {(EnableAuditLogging ? "Enabled" : "Disabled")}");
                
                StatusMessage = $"✅ System report generated: {reportPath}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Report generation failed: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task ExportDeviceReport()
        {
            StatusMessage = "📋 Exporting device report...";
            try
            {
                var devices = await _deviceService.GetAllDevicesAsync();
                var exportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                    $"DeviceReport_{DateTime.Now:yyyyMMdd}.csv");
                
                using var writer = new StreamWriter(exportPath);
                
                // Header
                await writer.WriteLineAsync("DeviceCode,DeviceName,Manufacturer,Model,Category,Department,Status,RiskClass,PurchasePrice,WarrantyExpiry");
                
                foreach (var device in devices)
                {
                    await writer.WriteLineAsync(
                        $"{device.DeviceCode},{device.DeviceName},{device.Manufacturer},{device.Model}," +
                        $"{device.Category},{device.Department},{device.Status},{device.RiskClassification}," +
                        $"{device.PurchasePrice},{device.WarrantyExpiryDate:yyyy-MM-dd}");
                }
                
                StatusMessage = $"✅ Device report exported: {exportPath} ({devices.Count()} devices)";
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Device export failed: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task ExportStaffReport()
        {
            StatusMessage = "👥 Exporting staff report...";
            try
            {
                var staff = await _staffService.GetAllStaffAsync();
                var exportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                    $"StaffReport_{DateTime.Now:yyyyMMdd}.csv");
                
                using var writer = new StreamWriter(exportPath);
                
                // Header
                await writer.WriteLineAsync("EmployeeId,FullName,Username,Email,Role,Department,JobTitle,IsActive,IsLocked,HireDate");
                
                foreach (var s in staff)
                {
                    await writer.WriteLineAsync(
                        $"{s.EmployeeId},{s.FullName},{s.Username},{s.Email}," +
                        $"{s.Role},{s.Department},{s.JobTitle},{s.IsActive},{s.IsLocked},{s.HireDate:yyyy-MM-dd}");
            }
            
            StatusMessage = $"✅ Staff report exported: {exportPath} ({staff.Count()} members)";
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Staff export failed: {ex.Message}";
            }
        }

        private static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private void ClearForm()
        {
            ShowStaffForm = false;
            SelectedStaff = null;
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
            SelectedRole = StaffRole.Staff;
            Department = string.Empty;
            JobTitle = string.Empty;
            Specialization = string.Empty;
            CanManageDevices = false;
            CanManageMaintenance = false;
            CanManageCalibration = false;
            CanViewReports = false;
            CanAccessAdminPanel = false;
            CanManageSpareParts = false;
            CanManageNetworkDevices = false;
            CanManageStaff = false;
            FormTitleText = "Add New Staff";
            FormSubtitle = "Create a new staff account";
            FormIcon = "➕";
            IsEditing = false;
        }

        [RelayCommand]
        private async Task PrintReport()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                DefaultExt = ".txt",
                FileName = $"AdminPanel_Report_{DateTime.Now:yyyyMMdd_HHmmss}.txt",
                Title = "Export Report"
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                var sb = new System.Text.StringBuilder();
                var now = DateTime.Now;

                // Header
                sb.AppendLine("═══════════════════════════════════════════════════════");
                sb.AppendLine("   MOMEN MEDICAL EQUIPMENT MANAGEMENT SYSTEM");
                sb.AppendLine("   Admin Control Panel - Report");
                sb.AppendLine("═══════════════════════════════════════════════════════");
                sb.AppendLine($"Generated: {now:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine($"Hospital:  {HospitalName}");
                sb.AppendLine($"Admin:     {AdministratorName}");
                sb.AppendLine($"License:   {InputLicenseNumber}");
                sb.AppendLine();

                // Section 1: Staff Overview
                sb.AppendLine("───────────────────────────────────────────────────────");
                sb.AppendLine("  STAFF OVERVIEW");
                sb.AppendLine("───────────────────────────────────────────────────────");
                sb.AppendLine($"Total Staff:        {TotalStaff}");
                sb.AppendLine($"Active Accounts:    {ActiveAccounts}");
                sb.AppendLine($"Locked Accounts:    {LockedAccounts}");
                sb.AppendLine($"Administrators:     {AdminCount}");
                sb.AppendLine($"Technicians:        {TechnicianCount}");
                sb.AppendLine($"Report Writers:     {ReportWriterCount}");
                sb.AppendLine($"Physicians:         {Physicians.Count}");
                sb.AppendLine($"Nurses:             {Nurses.Count}");
                sb.AppendLine();

                // Section 2: Staff Details
                sb.AppendLine("───────────────────────────────────────────────────────");
                sb.AppendLine("  STAFF DIRECTORY");
                sb.AppendLine("───────────────────────────────────────────────────────");
                sb.AppendLine($"{-10, -25} {-15} {-20} {-15}");
                sb.AppendLine(new string('-', 85));

                foreach (var s in AllStaff.OrderBy(x => x.LastName).ThenBy(x => x.FirstName))
                {
                    sb.AppendLine($"{s.EmployeeId,-10} {s.FullName,-25} {s.Role,-15} {s.Department,-20} {(s.IsLocked ? "Locked" : "Active"),-15}");
                }
                sb.AppendLine();

                // Section 3: License Info
                sb.AppendLine("───────────────────────────────────────────────────────");
                sb.AppendLine("  LICENSE INFORMATION");
                sb.AppendLine("───────────────────────────────────────────────────────");
                sb.AppendLine($"Total Licenses:     {TotalLicenses}");
                sb.AppendLine($"Active Licenses:    {ActiveLicenses}");
                sb.AppendLine($"License Status:     {LicenseStatusText}");
                sb.AppendLine($"License Type:       {SelectedLicenseTypeIndex switch { 0 => "3 Months (90 days)", 1 => "1 Year (365 days)", _ => "Lifetime (Unlimited)" }}");
                sb.AppendLine();

                // Section 4: System Info
                sb.AppendLine("───────────────────────────────────────────────────────");
                sb.AppendLine("  SYSTEM STATUS");
                sb.AppendLine("───────────────────────────────────────────────────────");
                sb.AppendLine($"Version:            {SystemVersion}");
                sb.AppendLine($"Database Size:      {DatabaseSize}");
                sb.AppendLine($"Disk Space:         {DiskSpace}");
                sb.AppendLine($"System Uptime:      {SystemUptime}");
                sb.AppendLine($"Memory Usage:       {MemoryUsage}");
                sb.AppendLine($"CPU Usage:          {CpuUsage}");
                sb.AppendLine($"Active Sessions:    {ActiveSessions}");
                sb.AppendLine($"Total Audit Logs:   {TotalAuditLogs}");
                sb.AppendLine($"Total Backups:      {TotalBackups}");
                sb.AppendLine();

                // Section 5: Security Settings
                sb.AppendLine("───────────────────────────────────────────────────────");
                sb.AppendLine("  SECURITY SETTINGS");
                sb.AppendLine("───────────────────────────────────────────────────────");
                sb.AppendLine($"Session Timeout:        {SessionTimeoutMinutes} minutes");
                sb.AppendLine($"Max Failed Logins:      {MaxFailedLoginAttempts}");
                sb.AppendLine($"Password Expiry:        {PasswordExpiryDays} days");
                sb.AppendLine($"Strong Passwords:       {(RequireStrongPasswords ? "Enabled" : "Disabled")}");
                sb.AppendLine($"Audit Logging:          {(EnableAuditLogging ? "Enabled" : "Disabled")}");
                sb.AppendLine($"Failed Logins (24h):    {FailedLogins24h}");
                sb.AppendLine();

                // Footer
                sb.AppendLine("═══════════════════════════════════════════════════════");
                sb.AppendLine("  End of Report");
                sb.AppendLine($"  Generated by {CurrentUserName}");
                sb.AppendLine("  © 2026 Momen Systems Company");
                sb.AppendLine("═══════════════════════════════════════════════════════");

                await File.WriteAllTextAsync(dialog.FileName, sb.ToString());

                StatusMessage = $"📄 Report exported: {Path.GetFileName(dialog.FileName)}";

                // Open the file in default text editor for printing
                if (await _dialogService.ShowConfirmAsync(
                    $"Report saved to:\n{dialog.FileName}\n\nOpen file for printing?",
                    "Open Report"))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = dialog.FileName,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Export error: {ex.Message}";
                await _dialogService.ShowMessageAsync($"Failed to generate report:\n{ex.Message}", "Export Error");
            }
        }

        private static string HashPassword(string password)
        {
            return Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(password)));
        }

        private static string GenerateSecurePassword()
        {
            const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            const string lower = "abcdefghijkmnopqrstuvwxyz";
            const string digits = "23456789";
            const string special = "!@#$%^&*";
            const string all = upper + lower + digits + special;

            var rng = new Random();
            var chars = new char[12];
            
            // Ensure at least one of each type
            chars[0] = upper[rng.Next(upper.Length)];
            chars[1] = lower[rng.Next(lower.Length)];
            chars[2] = digits[rng.Next(digits.Length)];
            chars[3] = special[rng.Next(special.Length)];
            
            // Fill the rest randomly
            for (int i = 4; i < chars.Length; i++)
            {
                chars[i] = all[rng.Next(all.Length)];
            }

            // Shuffle
            for (int i = chars.Length - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (chars[i], chars[j]) = (chars[j], chars[i]);
            }

            return new string(chars);
        }
    }
}
