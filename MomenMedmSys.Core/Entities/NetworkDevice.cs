using System;
using System.Collections.Generic;

namespace MomenMedmSys.Core.Entities
{
    /// <summary>
    /// Network-connected medical device with remote management capabilities
    /// </summary>
    public class NetworkDevice : BaseEntity
    {
        // Link to MedicalDevice (optional - discovered devices may not be in registry yet)
        public int? MedicalDeviceId { get; set; }
        public MedicalDevice? MedicalDevice { get; set; }

        // Network Identification
        public string DeviceName { get; set; } = string.Empty;
        public string Hostname { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string MacAddress { get; set; } = string.Empty;
        public int Port { get; set; }
        public string SubnetMask { get; set; } = string.Empty;
        public string Gateway { get; set; } = string.Empty;
        public string DnsServer { get; set; } = string.Empty;

        // Device Information
        public string Manufacturer { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string FirmwareVersion { get; set; } = string.Empty;
        public string SoftwareVersion { get; set; } = string.Empty;
        public string OperatingSystem { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty; // Imaging, Monitoring, Lab, etc.

        // Connection Status
        public DeviceConnectionStatus ConnectionStatus { get; set; } = DeviceConnectionStatus.Offline;
        public DateTime LastSeen { get; set; }
        public DateTime FirstDiscovered { get; set; } = DateTime.Now;
        public int ResponseTimeMs { get; set; } // Ping response time
        public int UptimeHours { get; set; }
        public string LastErrorMessage { get; set; } = string.Empty;
        public DateTime LastErrorTime { get; set; }

        // Remote Management
        public bool RemoteManagementEnabled { get; set; } = true;
        public string ManagementProtocol { get; set; } = string.Empty; // SSH, SNMP, HTTP, proprietary
        public string ManagementPort { get; set; } = string.Empty;
        public bool SupportsRemoteUpdate { get; set; }
        public bool SupportsRemoteDiagnostics { get; set; }
        public bool SupportsRemoteConfiguration { get; set; }
        public bool SupportsRemoteReboot { get; set; }
        public string RemoteAccessUrl { get; set; } = string.Empty;

        // Security
        public string AuthenticationMethod { get; set; } = string.Empty; // None, Basic, Certificate, OAuth
        public bool SslEnabled { get; set; }
        public string CertificateExpiry { get; set; } = string.Empty;
        public bool FirewallEnabled { get; set; }
        public string OpenPorts { get; set; } = string.Empty; // Comma-separated list

        // Monitoring
        public double CpuUsage { get; set; }
        public double MemoryUsage { get; set; }
        public double DiskUsage { get; set; }
        public double NetworkBandwidthUsage { get; set; }
        public int ActiveConnections { get; set; }
        public string ErrorLogs { get; set; } = string.Empty;
        public string SystemHealthStatus { get; set; } = string.Empty; // Healthy, Warning, Critical

        // Location
        public string Location { get; set; } = string.Empty; // Building/Room
        public string Department { get; set; } = string.Empty;

        // Discovery Method
        public DiscoveryMethod DiscoveredVia { get; set; } = DiscoveryMethod.Manual;
        public string DiscoveryProtocol { get; set; } = string.Empty; // mDNS, SNMP, WSD, etc.

        // Notes
        public string Notes { get; set; } = string.Empty;

        // Computed
        public bool IsOnline => ConnectionStatus == DeviceConnectionStatus.Online;
        public bool IsResponsive => ResponseTimeMs > 0 && ResponseTimeMs < 5000;
        public bool HasErrors => ConnectionStatus == DeviceConnectionStatus.Error;

        public string ConnectionStatusDisplay
        {
            get
            {
                return ConnectionStatus switch
                {
                    DeviceConnectionStatus.Online => $"Online ({ResponseTimeMs}ms)",
                    DeviceConnectionStatus.Offline => "Offline",
                    DeviceConnectionStatus.Warning => $"Warning ({ResponseTimeMs}ms)",
                    DeviceConnectionStatus.Error => $"Error: {LastErrorMessage}",
                    _ => "Unknown"
                };
            }
        }

        // Navigation
        public ICollection<DeviceActionLog> ActionLogs { get; set; } = new List<DeviceActionLog>();
    }

    /// <summary>
    /// Log of remote actions performed on network devices
    /// </summary>
    public class DeviceActionLog : BaseEntity
    {
        public int NetworkDeviceId { get; set; }
        public NetworkDevice NetworkDevice { get; set; } = null!;

        public string ActionType { get; set; } = string.Empty; // Update, Reboot, Configure, Diagnostic, etc.
        public string ActionDescription { get; set; } = string.Empty;
        public string Parameters { get; set; } = string.Empty; // JSON of action parameters
        public DeviceActionResult Result { get; set; } = DeviceActionResult.Pending;
        public string ResultMessage { get; set; } = string.Empty;
        public string ExecutedBy { get; set; } = string.Empty;
        public DateTime? CompletedAt { get; set; }
    }

    public enum DeviceConnectionStatus
    {
        Online = 1,
        Offline = 2,
        Warning = 3,
        Error = 4
    }

    public enum DiscoveryMethod
    {
        Manual = 1,
        NetworkScan = 2,
        mDNS = 3,
        SNMP = 4,
        WSD = 5,
        ARP = 6,
        Imported = 7
    }

    public enum DeviceActionResult
    {
        Pending = 1,
        Success = 2,
        Failed = 3,
        InProgress = 4,
        Cancelled = 5
    }

    public enum RemoteActionType
    {
        Ping = 1,
        GetStatus = 2,
        GetDiagnostics = 3,
        Reboot = 4,
        UpdateFirmware = 5,
        UpdateSoftware = 6,
        GetConfiguration = 7,
        SetConfiguration = 8,
        RestartService = 9,
        GetLogs = 10,
        ClearLogs = 11,
        Backup = 12,
        Restore = 13,
        Shutdown = 14,
        Custom = 99
    }
}
