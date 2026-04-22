namespace MomenMedmSys.Core.Enums
{
    /// <summary>
    /// Priority levels for notifications
    /// </summary>
    public enum NotificationPriority
    {
        /// <summary>Informational, no action required</summary>
        Low = 0,
        /// <summary>Attention needed within normal timeframes</summary>
        Medium = 1,
        /// <summary>Urgent, action required soon</summary>
        High = 2,
        /// <summary>Critical, immediate action required</summary>
        Critical = 3
    }
}
