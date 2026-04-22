using System;
using System.Collections.ObjectModel;
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
    public partial class UserFormViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly IDialogService _dialogService;
        private readonly CurrentUserService _currentUserService;
        private bool _isEditMode;
        private int _userId;

        public event Action? Saved;

        public UserFormViewModel(IAuthService authService, IDialogService dialogService,
            CurrentUserService currentUserService)
        {
            _authService = authService;
            _dialogService = dialogService;
            _currentUserService = currentUserService;
        }

        public ObservableCollection<string> Roles { get; } = new()
        {
            nameof(UserRole.Admin),
            nameof(UserRole.Manager),
            nameof(UserRole.Technician),
            nameof(UserRole.Viewer)
        };

        [ObservableProperty] private string _username = string.Empty;
        [ObservableProperty] private string _fullName = string.Empty;
        [ObservableProperty] private string _email = string.Empty;
        [ObservableProperty] private string _password = string.Empty;
        [ObservableProperty] private string _confirmPassword = string.Empty;
        [ObservableProperty] private string _selectedRole = nameof(UserRole.Viewer);
        [ObservableProperty] private bool _isActive = true;
        [ObservableProperty] private bool _isEditing;

        public void SetAddMode()
        {
            _isEditMode = false;
            IsEditing = false;
            Title = "Create User";
            Username = string.Empty;
            FullName = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            SelectedRole = nameof(UserRole.Viewer);
            IsActive = true;
            StatusMessage = "Fill in the user details and click Save.";
        }

        public void SetEditMode(User user)
        {
            _isEditMode = true;
            _userId = user.Id;
            IsEditing = true;
            Title = "Edit User";
            Username = user.Username;
            FullName = user.FullName;
            Email = user.Email;
            SelectedRole = user.Role.ToString();
            IsActive = user.IsActive;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            StatusMessage = $"Editing user: {user.Username}";
        }

        [RelayCommand]
        private async Task SaveUser()
        {
            // Validation
            if (string.IsNullOrWhiteSpace(Username))
            {
                StatusMessage = "Username is required.";
                return;
            }

            if (string.IsNullOrWhiteSpace(FullName))
            {
                StatusMessage = "Full name is required.";
                return;
            }

            if (!_isEditMode && string.IsNullOrWhiteSpace(Password))
            {
                StatusMessage = "Password is required for new users.";
                return;
            }

            if (!string.IsNullOrEmpty(Password) && Password != ConfirmPassword)
            {
                StatusMessage = "Passwords do not match.";
                return;
            }

            if (!string.IsNullOrEmpty(Password) && Password.Length < 6)
            {
                StatusMessage = "Password must be at least 6 characters.";
                return;
            }

            if (!Enum.TryParse<UserRole>(SelectedRole, out var role))
            {
                StatusMessage = "Invalid role selected.";
                return;
            }

            try
            {
                if (_isEditMode)
                {
                    var user = new User
                    {
                        Id = _userId,
                        Username = Username,
                        FullName = FullName,
                        Email = Email,
                        Role = role,
                        IsActive = IsActive
                    };

                    await _authService.UpdateUserAsync(user);

                    // Reset password if provided
                    if (!string.IsNullOrEmpty(Password))
                    {
                        await _authService.ResetPasswordAsync(_userId, Password);
                    }

                    StatusMessage = $"User '{Username}' updated successfully.";
                }
                else
                {
                    var user = new User
                    {
                        Username = Username,
                        FullName = FullName,
                        Email = Email,
                        Role = role,
                        CreatedBy = _currentUserService.CurrentUser?.Username ?? "System"
                    };

                    await _authService.CreateUserAsync(user, Password);
                    StatusMessage = $"User '{Username}' created successfully.";
                }

                Saved?.Invoke();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving user: {ex.Message}";
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            Saved?.Invoke();
        }
    }
}
