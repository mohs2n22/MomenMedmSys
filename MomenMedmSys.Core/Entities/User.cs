using System;
using MomenMedmSys.Core.Enums;

namespace MomenMedmSys.Core.Entities
{
    /// <summary>
    /// System user entity for authentication and authorization
    /// </summary>
    public class User : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Viewer;
        public bool IsLocked { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? PasswordExpiryDate { get; set; }

        // Navigation
        public ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
    }
}
