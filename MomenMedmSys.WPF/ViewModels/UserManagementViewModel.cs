using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Core.Enums;
using MomenMedmSys.Services;
using MomenMedmSys.WPF.Services;
using MomenMedmSys.WPF.ViewModels.Base;

namespace MomenMedmSys.WPF.ViewModels
{
    public partial class UserManagementViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly IDialogService _dialogService;
        private readonly CurrentUserService _currentUserService;
        private MainViewModel? _mainVM;

        public UserManagementViewModel(IAuthService authService, IDialogService dialogService,
            CurrentUserService currentUserService)
        {
            _authService = authService;
            _dialogService = dialogService;
            _currentUserService = currentUserService;
            Title = "User Management";
            LoadUsersCommand.Execute(null);
        }

        public void SetMainViewModel(MainViewModel mainVM)
        {
            _mainVM = mainVM;
        }

        public ObservableCollection<User> Users { get; } = new();
        public ObservableCollection<User> FilteredUsers { get; } = new();

        private User? _selectedUser;
        public User? SelectedUser
        {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value);
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    ApplyFilter();
            }
        }

        private string _filterRole = "All";
        public string FilterRole
        {
            get => _filterRole;
            set
            {
                if (SetProperty(ref _filterRole, value))
                    ApplyFilter();
            }
        }

        [RelayCommand]
        private async Task LoadUsers()
        {
            IsLoading = true;
            try
            {
                Users.Clear();
                var allUsers = await _authService.GetAllUsersAsync();
                foreach (var user in allUsers)
                    Users.Add(user);
                ApplyFilter();
                StatusMessage = $"Loaded {Users.Count} users";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading users: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplyFilter()
        {
            FilteredUsers.Clear();
            var query = Users.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var search = SearchText.ToLower();
                query = query.Where(u =>
                    u.Username.ToLower().Contains(search) ||
                    u.FullName.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search));
            }

            if (FilterRole != "All")
            {
                if (Enum.TryParse<UserRole>(FilterRole, out var role))
                {
                    query = query.Where(u => u.Role == role);
                }
            }

            foreach (var user in query)
                FilteredUsers.Add(user);
        }

        [RelayCommand]
        private void AddUser()
        {
            var form = new UserFormViewModel(_authService, _dialogService, _currentUserService);
            form.SetAddMode();
            form.Saved += OnUserSaved;
            _mainVM?.NavigateTo(form);
        }

        [RelayCommand]
        private async Task EditUser()
        {
            if (SelectedUser == null)
            {
                await _dialogService.ShowMessageAsync("Please select a user to edit.", "No Selection");
                return;
            }

            var form = new UserFormViewModel(_authService, _dialogService, _currentUserService);
            form.SetEditMode(SelectedUser);
            form.Saved += OnUserSaved;
            _mainVM?.NavigateTo(form);
        }

        [RelayCommand]
        private async Task DeleteUser()
        {
            if (SelectedUser == null) return;

            if (SelectedUser.Id == _currentUserService.UserId)
            {
                await _dialogService.ShowMessageAsync("You cannot delete your own account.", "Access Denied");
                return;
            }

            var confirmed = await _dialogService.ShowConfirmAsync(
                $"Are you sure you want to delete user '{SelectedUser.Username}'?", "Confirm Delete");

            if (confirmed)
            {
                try
                {
                    await _authService.DeleteUserAsync(SelectedUser.Id);
                    Users.Remove(SelectedUser);
                    ApplyFilter();
                    SelectedUser = null;
                    StatusMessage = "User deleted";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Error deleting user: {ex.Message}";
                }
            }
        }

        [RelayCommand]
        private async Task ToggleUserLock()
        {
            if (SelectedUser == null) return;

            if (SelectedUser.Id == _currentUserService.UserId)
            {
                await _dialogService.ShowMessageAsync("You cannot lock/unlock your own account.", "Access Denied");
                return;
            }

            try
            {
                if (SelectedUser.IsLocked)
                {
                    await _authService.UnlockAccountAsync(SelectedUser.Id);
                    StatusMessage = $"User '{SelectedUser.Username}' unlocked";
                }
                else
                {
                    await _authService.LockAccountAsync(SelectedUser.Id);
                    StatusMessage = $"User '{SelectedUser.Username}' locked";
                }
                await LoadUsers();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task ResetUserPassword()
        {
            if (SelectedUser == null) return;

            if (SelectedUser.Id == _currentUserService.UserId)
            {
                await _dialogService.ShowMessageAsync("Use 'Change Password' from your profile to change your own password.", "Info");
                return;
            }

            var newPassword = "TempPass123!";
            var confirmed = await _dialogService.ShowConfirmAsync(
                $"Reset password for '{SelectedUser.Username}' to '{newPassword}'?", "Reset Password");

            if (confirmed)
            {
                try
                {
                    await _authService.ResetPasswordAsync(SelectedUser.Id, newPassword);
                    StatusMessage = $"Password reset for '{SelectedUser.Username}'";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Error resetting password: {ex.Message}";
                }
            }
        }

        private void OnUserSaved()
        {
            LoadUsersCommand.Execute(null);
        }
    }
}
