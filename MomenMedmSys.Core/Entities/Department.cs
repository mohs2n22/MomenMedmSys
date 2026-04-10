using System;
using System.Collections.Generic;

namespace MomenMedmSys.Core.Entities
{
    /// <summary>
    /// Department entity - organizational unit where devices are assigned
    /// </summary>
    public class Department : BaseEntity
    {
        public string DepartmentCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Manager { get; set; } = string.Empty;
        public string Building { get; set; } = string.Empty;
        public string Floor { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public decimal Budget { get; set; }

        // Device counts
        public int DeviceCount { get; set; }
        public int ActiveDeviceCount { get; set; }

        // Navigation
        public ICollection<MedicalDevice> Devices { get; set; } = new List<MedicalDevice>();
        public ICollection<StaffMember> StaffMembers { get; set; } = new List<StaffMember>();
    }
}
