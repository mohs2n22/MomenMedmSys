namespace MomenMedmSys.Core.Enums
{
    /// <summary>
    /// System user roles for role-based access control
    /// </summary>
    public enum UserRole
    {
        /// <summary>Full system access, can manage users and all settings</summary>
        Admin = 0,
        /// <summary>Can manage devices, maintenance, and view reports</summary>
        Manager = 1,
        /// <summary>Can perform maintenance, calibration, and work order tasks</summary>
        Technician = 2,
        /// <summary>Read-only access to dashboards and reports</summary>
        Viewer = 3
    }
}
