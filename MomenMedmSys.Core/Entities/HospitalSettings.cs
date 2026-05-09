using System;

namespace MomenMedmSys.Core.Entities
{
    public class HospitalSettings : BaseEntity
    {
        public string HospitalName { get; set; } = string.Empty;
        public string LogoPath { get; set; } = string.Empty;
    }
}