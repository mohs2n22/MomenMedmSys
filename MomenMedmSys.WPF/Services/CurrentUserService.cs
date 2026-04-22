using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Core.Enums;
using MomenMedmSys.Services;

namespace MomenMedmSys.WPF.Services
{
    /// <summary>
    /// Singleton service that holds the currently authenticated user.
    /// Does not depend on scoped services in constructor to avoid captive dependency issues.
    /// </summary>
    public class CurrentUserService
    {
        private readonly IServiceProvider _serviceProvider;

        public CurrentUserService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public int? UserId { get; private set; }
        public User? CurrentUser { get; private set; }
        public bool IsAuthenticated => CurrentUser != null && CurrentUser.IsActive;

        public UserRole CurrentRole => CurrentUser?.Role ?? UserRole.Viewer;

        public bool IsAdmin => CurrentRole == UserRole.Admin;
        public bool IsManagerOrAbove => CurrentRole == UserRole.Admin || CurrentRole == UserRole.Manager;

        public void SetUser(User user)
        {
            CurrentUser = user;
            UserId = user.Id;
        }

        public async Task LogoutAsync()
        {
            if (UserId.HasValue)
            {
                using var scope = _serviceProvider.CreateScope();
                var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                await authService.LogoutAsync(UserId.Value);
            }
            CurrentUser = null;
            UserId = null;
        }

        public async Task RefreshCurrentUserAsync()
        {
            if (UserId.HasValue)
            {
                using var scope = _serviceProvider.CreateScope();
                var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                CurrentUser = await authService.GetCurrentUserAsync(UserId.Value);
            }
        }

        /// <summary>
        /// Check if the current user has at least the specified role level
        /// </summary>
        public bool HasRole(UserRole requiredRole)
        {
            if (!IsAuthenticated) return false;
            return (int)CurrentRole <= (int)requiredRole; // lower enum value = higher privilege
        }
    }
}
