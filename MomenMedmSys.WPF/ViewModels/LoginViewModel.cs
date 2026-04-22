using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MomenMedmSys.Services;
using MomenMedmSys.WPF.Services;

namespace MomenMedmSys.WPF.ViewModels
{
    public partial class LoginViewModel : ObservableObject, IDataErrorInfo
    {
        private readonly IAuthService _authService;
        private readonly CurrentUserService _currentUserService;

        public LoginViewModel(IAuthService authService, CurrentUserService currentUserService)
        {
            _authService = authService;
            _currentUserService = currentUserService;
        }

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private bool _rememberMe;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public string this[string columnName] => string.Empty;
        public string Error => string.Empty;

        [RelayCommand]
        private async Task Login()
        {
            System.Diagnostics.Debug.WriteLine("[LoginVM] Login called");
            System.Diagnostics.Debug.WriteLine($"[LoginVM] User='{Username}' Pass len={Password?.Length ?? 0}");
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter both username and password.";
                System.Diagnostics.Debug.WriteLine($"[LoginVM] Error: {ErrorMessage}");
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                System.Diagnostics.Debug.WriteLine("[LoginVM] Calling AuthenticateAsync...");
                var user = await _authService.AuthenticateAsync(Username, Password);
                System.Diagnostics.Debug.WriteLine($"[LoginVM] Auth result: {(user == null ? "null" : user.Username)}");

                if (user == null)
                {
                    ErrorMessage = "Invalid username or password. Account may be locked.";
                    System.Diagnostics.Debug.WriteLine($"[LoginVM] Error: {ErrorMessage}");
                    return;
                }

                _currentUserService.SetUser(user);
                System.Diagnostics.Debug.WriteLine("[LoginVM] User set, closing dialog...");

                // Signal success by closing the login window
                if (Application.Current.Windows.Count > 0)
                {
                    // Find and close the login window
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window is Views.LoginView)
                        {
                            window.DialogResult = true;
                            window.Close();
                            break;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Login failed: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"[LoginVM] Exception: {ex}");
                MessageBox.Show(ErrorMessage, "MomenMedmSys - Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
