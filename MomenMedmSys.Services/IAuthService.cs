using System.Collections.Generic;
using System.Threading.Tasks;
using MomenMedmSys.Core.Entities;

namespace MomenMedmSys.Services
{
    /// <summary>
    /// Service for user authentication and session management — BCrypt password hashing, login/logout,
    /// password change/reset, account lock/unlock, user CRUD, and session tracking.
    /// Implements role-based access control with secure password storage.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticate user with username and password. Returns User if successful, null otherwise.
        /// </summary>
        Task<User?> AuthenticateAsync(string username, string password);

        /// <summary>
        /// Log out a user by ending their active session
        /// </summary>
        Task LogoutAsync(int userId);

        /// <summary>
        /// Change password for a user (requires old password)
        /// </summary>
        Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);

        /// <summary>
        /// Reset password (admin operation, no old password required)
        /// </summary>
        Task<bool> ResetPasswordAsync(int userId, string newPassword);

        /// <summary>
        /// Lock a user account
        /// </summary>
        Task LockAccountAsync(int userId);

        /// <summary>
        /// Unlock a user account and reset failed login attempts
        /// </summary>
        Task UnlockAccountAsync(int userId);

        /// <summary>
        /// Get all users in the system
        /// </summary>
        Task<IEnumerable<User>> GetAllUsersAsync();

        /// <summary>
        /// Create a new user with the given password
        /// </summary>
        Task<User> CreateUserAsync(User user, string password);

        /// <summary>
        /// Update an existing user's details (excluding password)
        /// </summary>
        Task<User> UpdateUserAsync(User user);

        /// <summary>
        /// Soft-delete a user by setting IsActive to false
        /// </summary>
        Task DeleteUserAsync(int userId);

        /// <summary>
        /// Get the currently authenticated user (from session context)
        /// </summary>
        Task<User?> GetCurrentUserAsync(int? userId = null);

        /// <summary>
        /// Validate a password against a BCrypt hash
        /// </summary>
        bool IsPasswordValid(string password, string hash);

        /// <summary>
        /// Hash a password using BCrypt with work factor 12
        /// </summary>
        string HashPassword(string password);
    }
}
