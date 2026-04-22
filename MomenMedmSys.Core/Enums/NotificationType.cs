namespace MomenMedmSys.Core.Enums
{
    /// <summary>
    /// Types of system notifications
    /// </summary>
    public enum NotificationType
    {
        /// <summary>Maintenance due or overdue</summary>
        Maintenance = 0,
        /// <summary>Calibration due or overdue</summary>
        Calibration = 1,
        /// <summary>Warranty expiring soon</summary>
        Warranty = 2,
        /// <summary>Low stock or reorder needed</summary>
        Stock = 3,
        /// <summary>Critical risk incident</summary>
        Risk = 4,
        /// <summary>System-generated informational message</summary>
        System = 5
    }
}
