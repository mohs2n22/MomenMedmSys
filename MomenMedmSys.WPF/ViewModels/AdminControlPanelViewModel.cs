using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Services;
using MomenMedmSys.WPF.Services;
using MomenMedmSys.WPF.ViewModels.Base;

namespace MomenMedmSys.WPF.ViewModels
{
    public partial class AdminControlPanelViewModel : ViewModelBase
    {
        private readonly IStaffManagementService _staffService;
        private readonly IDialogService _dialogService;

        public AdminControlPanelViewModel(IStaffManagementService staffService, IDialogService dialogService)
        {
            _staffService = staffService;
            _dialogService = dialogService;
            Title = "Admin Control Panel";
            LoadAllCommand.Execute(null);
        }

        // User lists by type
        public ObservableCollection<StaffMember> AllStaff { get; } = new();
        public ObservableCollection<StaffMember> Administrators { get; } = new();
        public ObservableCollection<StaffMember> HardwareTechnicians { get; } = new();
        public ObservableCollection<StaffMember> ReportWriters { get; } = new();
        public ObservableCollection<StaffMember> Physicians { get; } = new();
        public ObservableCollection<StaffMember> Nurses { get; } = new();

        // Stats
        [ObservableProperty] private int _totalStaff;
        [ObservableProperty] private int _activeAccounts;
        [ObservableProperty] private int _lockedAccounts;
        [ObservableProperty] private int _adminCount;
        [ObservableProperty] private int _technicianCount;
        [ObservableProperty] private int _reportWriterCount;

        // Selected items
        [ObservableProperty] private StaffMember? _selectedStaff;
        [ObservableProperty] private int _activeTabIndex;

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

        public bool IsEditing => SelectedStaff != null;

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

                TotalStaff = AllStaff.Count;
                ActiveAccounts = await _staffService.GetActiveAccountCountAsync();
                LockedAccounts = await _staffService.GetLockedAccountCountAsync();
                AdminCount = Administrators.Count;
                TechnicianCount = HardwareTechnicians.Count;
                ReportWriterCount = ReportWriters.Count;

                StatusMessage = $"Loaded {TotalStaff} staff members";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally { IsLoading = false; }
        }

        [RelayCommand]
        private void StartAddNew()
        {
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
            OnPropertyChanged(nameof(IsEditing));
        }

        [RelayCommand]
        private void StartEdit(StaffMember? staff)
        {
            if (staff == null) return;
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
            OnPropertyChanged(nameof(IsEditing));
        }

        [RelayCommand]
        private async Task SaveStaff()
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
            {
                await _dialogService.ShowMessageAsync("First name and last name are required.", "Validation Error");
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
                    SelectedStaff.UpdatedAt = DateTime.Now;

                    if (!string.IsNullOrWhiteSpace(Password))
                    {
                        SelectedStaff.PasswordHash = HashPassword(Password);
                    }

                    await _staffService.UpdateStaffAsync(SelectedStaff);
                    StatusMessage = $"Updated: {FirstName} {LastName}";
                }
                else
                {
                    // Create new
                    var staff = new StaffMember
                    {
                        EmployeeId = $"EMP-{DateTime.Now:yyyyMMdd}",
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
                        HireDate = DateTime.Now,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };

                    await _staffService.CreateStaffAsync(staff);
                    StatusMessage = $"Created: {FirstName} {LastName}";
                }

                ClearForm();
                await LoadAllCommand.ExecuteAsync(null);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Save error: {ex.Message}";
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
                $"Delete '{SelectedStaff.FullName}' ({SelectedStaff.Role})?", "Confirm Delete");

            if (confirmed)
            {
                await _staffService.DeleteStaffAsync(SelectedStaff.Id);
                StatusMessage = $"Deleted: {SelectedStaff.FullName}";
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

            var newPass = "Reset123!"; // Default - in production, generate random
            await _staffService.ResetPasswordAsync(SelectedStaff.Id, HashPassword(newPass));
            await _dialogService.ShowMessageAsync($"Password reset for {SelectedStaff.Username}.\nTemporary password: {newPass}", "Password Reset");
            StatusMessage = $"Password reset for {SelectedStaff.Username}";
        }

        [RelayCommand]
        private async Task ToggleLock()
        {
            if (SelectedStaff == null) return;
            await _staffService.ToggleAccountLockAsync(SelectedStaff.Id, !SelectedStaff.IsLocked);
            StatusMessage = $"Account {(SelectedStaff.IsLocked ? "locked" : "unlocked")}: {SelectedStaff.Username}";
            await LoadAllCommand.ExecuteAsync(null);
        }

        private void ClearForm()
        {
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
            OnPropertyChanged(nameof(IsEditing));
        }

        private static string HashPassword(string password)
        {
            // Simple hash - in production use BCrypt or similar
            return Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(password)));
        }
    }
}
