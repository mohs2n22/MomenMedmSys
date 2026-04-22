using System;
using MomenMedmSys.Core.Enums;

namespace MomenMedmSys.Core.Entities
{
    /// <summary>
    /// System notification for alerting users about important events
    /// </summary>
    public class Notification : BaseEntity
    {
        /// <summary>Target user ID (null = all users)</summary>
        public int? UserId { get; set; }
        public User? User { get; set; }

        /// <summary>Notification title</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Notification message body</summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>Type of notification</summary>
        public NotificationType Type { get; set; }

        /// <summary>Priority level</summary>
        public NotificationPriority Priority { get; set; }

        /// <summary>Whether the notification has been read</summary>
        public bool IsRead { get; set; }

        /// <summary>Timestamp when read</summary>
        public DateTime? ReadAt { get; set; }

        /// <summary>Related entity type (e.g., "MedicalDevice", "MaintenanceRecord")</summary>
        public string EntityType { get; set; } = string.Empty;

        /// <summary>Related entity ID</summary>
        public int? EntityId { get; set; }

        /// <summary>Navigation target URL or view identifier</summary>
        public string ActionUrl { get; set; } = string.Empty;

        /// <summary>Date by which action is required</summary>
        public DateTime DueDate { get; set; }
    }
}
