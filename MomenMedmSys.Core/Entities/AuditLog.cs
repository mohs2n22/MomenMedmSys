using System;

namespace MomenMedmSys.Core.Entities
{
    /// <summary>
    /// Audit log entry tracking changes to entities in the system.
    /// Provides a complete audit trail for compliance and accountability.
    /// </summary>
    public class AuditLog : BaseEntity
    {
        /// <summary>
        /// The type of entity that was changed (e.g., "MedicalDevice", "MaintenanceRecord").
        /// </summary>
        public string EntityType { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the entity that was changed.
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// The action performed: "Create", "Update", "Delete".
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the user who performed the action.
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// The username of the user who performed the action.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// JSON representation of the entity values before the change.
        /// </summary>
        public string? OldValues { get; set; }

        /// <summary>
        /// JSON representation of the entity values after the change.
        /// </summary>
        public string? NewValues { get; set; }

        /// <summary>
        /// Description of affected records (e.g., entity name or summary).
        /// </summary>
        public string? AffectedRecords { get; set; }

        /// <summary>
        /// IP address from which the action was performed. Defaults to "Local" for desktop.
        /// </summary>
        public string IpAddress { get; set; } = "Local";

        /// <summary>
        /// The timestamp when the action occurred.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
