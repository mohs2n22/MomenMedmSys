using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MomenMedmSys.Core.Entities;

namespace MomenMedmSys.Services
{
    /// <summary>
    /// Service for comprehensive audit trail logging — records every data change with entity type, action,
    /// user, timestamp, before/after values. Supports Excel export, entity history queries, and user activity tracking.
    /// </summary>
    public interface IAuditService
    {
        /// <summary>
        /// Log an audit entry for an entity change.
        /// </summary>
        Task LogAsync(string entityType, int entityId, string action, int? userId, string userName,
            string? oldValues = null, string? newValues = null, string? affectedRecords = null, string? ipAddress = null);

        /// <summary>
        /// Get audit logs filtered by entity type, entity ID, date range, and user.
        /// </summary>
        Task<IEnumerable<AuditLog>> GetAuditLogsAsync(string? entityType = null, int? entityId = null,
            DateTime? startDate = null, DateTime? endDate = null, int? userId = null);

        /// <summary>
        /// Get the full change history for a specific entity.
        /// </summary>
        Task<IEnumerable<AuditLog>> GetEntityHistoryAsync(string entityType, int entityId);

        /// <summary>
        /// Get activity for a specific user within a date range.
        /// </summary>
        Task<IEnumerable<AuditLog>> GetUserActivityAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Get the most recent audit entries.
        /// </summary>
        Task<IEnumerable<AuditLog>> GetRecentActivityAsync(int count = 50);

        /// <summary>
        /// Export all audit logs to an Excel file.
        /// </summary>
        Task ExportAuditLogsAsync(string filePath, string? entityType = null, int? entityId = null,
            DateTime? startDate = null, DateTime? endDate = null, int? userId = null);

        /// <summary>
        /// Get total count of all audit log entries.
        /// </summary>
        Task<int> GetTotalAuditLogCountAsync();
    }
}
