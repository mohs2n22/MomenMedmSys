using System;

namespace MomenMedmSys.Core.Entities
{
    /// <summary>
    /// Tracks user login sessions for audit and security
    /// </summary>
    public class UserSession
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime LoginTime { get; set; } = DateTime.Now;
        public DateTime? LogoutTime { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // Navigation
        public User? User { get; set; }
    }
}
