using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Data;

namespace MomenMedmSys.Services
{
    /// <summary>
    /// Service for ICMP ping-based network device discovery and remote monitoring — device CRUD,
    /// network scanning, response time measurement, status checking, remote action execution, and action logging.
    /// </summary>
    public interface INetworkDiscoveryService
    {
        Task<IEnumerable<NetworkDevice>> GetAllNetworkDevicesAsync();
        Task<NetworkDevice?> GetDeviceByIdAsync(int id);
        Task<NetworkDevice> AddDeviceAsync(NetworkDevice device);
        Task UpdateDeviceAsync(NetworkDevice device);
        Task DeleteDeviceAsync(int id);
        Task<List<NetworkDevice>> DiscoverNetworkAsync(string subnet = "");
        Task<bool> PingDeviceAsync(string ipAddress);
        Task<int> GetResponseTimeAsync(string ipAddress);
        Task<DeviceConnectionStatus> CheckDeviceStatusAsync(NetworkDevice device);
        Task RefreshAllDeviceStatusesAsync();
        Task<DeviceActionLog> ExecuteRemoteActionAsync(int deviceId, RemoteActionType action, string parameters = "", string executedBy = "System");
        Task<IEnumerable<DeviceActionLog>> GetActionLogsAsync(int deviceId);
        Task<int> GetOnlineCountAsync();
        Task<int> GetOfflineCountAsync();
        Task<int> GetWarningCountAsync();
    }

    public class NetworkDiscoveryService : INetworkDiscoveryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NetworkDiscoveryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<NetworkDevice>> GetAllNetworkDevicesAsync()
        {
            return await _unitOfWork.NetworkDevices.GetAllAsync();
        }

        public async Task<NetworkDevice?> GetDeviceByIdAsync(int id)
        {
            return await _unitOfWork.NetworkDevices.GetByIdAsync(id);
        }

        public async Task<NetworkDevice> AddDeviceAsync(NetworkDevice device)
        {
            device.FirstDiscovered = DateTime.Now;
            device.LastSeen = DateTime.Now;
            device.CreatedAt = DateTime.Now;

            // Initial status check
            device.ConnectionStatus = await CheckDeviceStatusAsync(device);
            if (device.IsOnline)
            {
                device.ResponseTimeMs = await GetResponseTimeAsync(device.IpAddress);
            }

            await _unitOfWork.NetworkDevices.AddAsync(device);
            await _unitOfWork.SaveChangesAsync();
            return device;
        }

        public async Task UpdateDeviceAsync(NetworkDevice device)
        {
            device.UpdatedAt = DateTime.Now;
            _unitOfWork.NetworkDevices.Update(device);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteDeviceAsync(int id)
        {
            var device = await _unitOfWork.NetworkDevices.GetByIdAsync(id);
            if (device != null)
            {
                _unitOfWork.NetworkDevices.Remove(device);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Scan the local network for active devices
        /// </summary>
        public async Task<List<NetworkDevice>> DiscoverNetworkAsync(string subnet = "")
        {
            var discovered = new List<NetworkDevice>();

            // Determine subnet to scan
            if (string.IsNullOrEmpty(subnet))
            {
                subnet = GetLocalSubnet();
            }

            // Scan IP range
            var ipBase = string.Join(".", subnet.Split('.').Take(3));
            var existingDevices = (await GetAllNetworkDevicesAsync()).ToList();

            for (int i = 1; i <= 254; i++)
            {
                var ip = $"{ipBase}.{i}";
                var ping = new Ping();
                try
                {
                    var reply = await ping.SendPingAsync(ip, 500);
                    if (reply.Status == IPStatus.Success)
                    {
                        // Check if already known
                        var existing = existingDevices.FirstOrDefault(d => d.IpAddress == ip);
                        if (existing == null)
                        {
                            discovered.Add(new NetworkDevice
                            {
                                IpAddress = ip,
                                MacAddress = string.Empty,
                                Hostname = string.Empty,
                                DeviceName = ip,
                                ConnectionStatus = DeviceConnectionStatus.Online,
                                ResponseTimeMs = (int)reply.RoundtripTime,
                                LastSeen = DateTime.Now,
                                DiscoveredVia = DiscoveryMethod.NetworkScan,
                                DiscoveryProtocol = "ICMP Ping"
                            });
                        }
                    }
                }
                catch
                {
                    // Host unreachable or timeout - skip
                }
            }

            return discovered;
        }

        public async Task<bool> PingDeviceAsync(string ipAddress)
        {
            try
            {
                var ping = new Ping();
                var reply = await ping.SendPingAsync(ipAddress, 2000);
                return reply.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> GetResponseTimeAsync(string ipAddress)
        {
            try
            {
                var ping = new Ping();
                var reply = await ping.SendPingAsync(ipAddress, 2000);
                return reply.Status == IPStatus.Success ? (int)reply.RoundtripTime : -1;
            }
            catch
            {
                return -1;
            }
        }

        public async Task<DeviceConnectionStatus> CheckDeviceStatusAsync(NetworkDevice device)
        {
            try
            {
                var ping = new Ping();
                var reply = await ping.SendPingAsync(device.IpAddress, 3000);

                if (reply.Status == IPStatus.Success)
                {
                    device.ResponseTimeMs = (int)reply.RoundtripTime;
                    device.LastSeen = DateTime.Now;
                    device.LastErrorMessage = string.Empty;

                    // Determine status based on response time
                    if (reply.RoundtripTime > 2000)
                        return DeviceConnectionStatus.Warning;
                    return DeviceConnectionStatus.Online;
                }

                return DeviceConnectionStatus.Offline;
            }
            catch (Exception ex)
            {
                device.LastErrorMessage = ex.Message;
                device.LastErrorTime = DateTime.Now;
                return DeviceConnectionStatus.Error;
            }
        }

        public async Task RefreshAllDeviceStatusesAsync()
        {
            var devices = await GetAllNetworkDevicesAsync();
            foreach (var device in devices)
            {
                device.ConnectionStatus = await CheckDeviceStatusAsync(device);
                _unitOfWork.NetworkDevices.Update(device);
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<DeviceActionLog> ExecuteRemoteActionAsync(int deviceId, RemoteActionType action, string parameters = "", string executedBy = "System")
        {
            var device = await _unitOfWork.NetworkDevices.GetByIdAsync(deviceId);
            if (device == null)
                throw new InvalidOperationException($"Device {deviceId} not found");

            if (!device.IsOnline)
                throw new InvalidOperationException($"Device {device.DeviceName} is offline");

            var log = new DeviceActionLog
            {
                NetworkDeviceId = deviceId,
                ActionType = action.ToString(),
                ActionDescription = GetActionDescription(action),
                Parameters = parameters,
                Result = DeviceActionResult.InProgress,
                ExecutedBy = executedBy,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.DeviceActionLogs.AddAsync(log);

            // Execute action (simulated - in production would use actual protocols)
            try
            {
                await ExecuteAction(device, action, parameters);
                log.Result = DeviceActionResult.Success;
                log.ResultMessage = "Action completed successfully";
            }
            catch (Exception ex)
            {
                log.Result = DeviceActionResult.Failed;
                log.ResultMessage = ex.Message;
            }

            log.CompletedAt = DateTime.Now;
            await _unitOfWork.SaveChangesAsync();

            return log;
        }

        public async Task<IEnumerable<DeviceActionLog>> GetActionLogsAsync(int deviceId)
        {
            return await _unitOfWork.DeviceActionLogs.FindAsync(a => a.NetworkDeviceId == deviceId);
        }

        public async Task<int> GetOnlineCountAsync()
        {
            var devices = await GetAllNetworkDevicesAsync();
            return devices.Count(d => d.IsOnline);
        }

        public async Task<int> GetOfflineCountAsync()
        {
            var devices = await GetAllNetworkDevicesAsync();
            return devices.Count(d => d.ConnectionStatus == DeviceConnectionStatus.Offline);
        }

        public async Task<int> GetWarningCountAsync()
        {
            var devices = await GetAllNetworkDevicesAsync();
            return devices.Count(d => d.ConnectionStatus == DeviceConnectionStatus.Warning);
        }

        // ===== Private Helpers =====

        private string GetLocalSubnet()
        {
            try
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var ni in interfaces)
                {
                    if (ni.OperationalStatus == OperationalStatus.Up &&
                        ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        var props = ni.GetIPProperties();
                        foreach (var addr in props.UnicastAddresses)
                        {
                            if (addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                var ipParts = addr.Address.ToString().Split('.');
                                return $"{ipParts[0]}.{ipParts[1]}.{ipParts[2]}.1";
                            }
                        }
                    }
                }
            }
            catch { }
            return "192.168.1.1"; // Default fallback
        }

        private string GetActionDescription(RemoteActionType action)
        {
            return action switch
            {
                RemoteActionType.Ping => "Ping device",
                RemoteActionType.GetStatus => "Get device status",
                RemoteActionType.GetDiagnostics => "Run diagnostics",
                RemoteActionType.Reboot => "Reboot device",
                RemoteActionType.UpdateFirmware => "Update firmware",
                RemoteActionType.UpdateSoftware => "Update software",
                RemoteActionType.GetConfiguration => "Get configuration",
                RemoteActionType.SetConfiguration => "Set configuration",
                RemoteActionType.RestartService => "Restart service",
                RemoteActionType.GetLogs => "Retrieve logs",
                RemoteActionType.ClearLogs => "Clear logs",
                RemoteActionType.Backup => "Backup configuration",
                RemoteActionType.Restore => "Restore configuration",
                RemoteActionType.Shutdown => "Shutdown device",
                RemoteActionType.Custom => "Custom action",
                _ => "Unknown action"
            };
        }

        private async Task ExecuteAction(NetworkDevice device, RemoteActionType action, string parameters)
        {
            // In production, this would use actual network protocols (SSH, SNMP, HTTP API, etc.)
            // For now, simulate with delays
            await Task.Delay(500);

            switch (action)
            {
                case RemoteActionType.Ping:
                    // Already handled by status check
                    break;

                case RemoteActionType.Reboot:
                    if (!device.SupportsRemoteReboot)
                        throw new InvalidOperationException("Device does not support remote reboot");
                    break;

                case RemoteActionType.UpdateFirmware:
                case RemoteActionType.UpdateSoftware:
                    if (!device.SupportsRemoteUpdate)
                        throw new InvalidOperationException("Device does not support remote updates");
                    break;

                case RemoteActionType.GetDiagnostics:
                    if (!device.SupportsRemoteDiagnostics)
                        throw new InvalidOperationException("Device does not support remote diagnostics");
                    break;

                case RemoteActionType.GetConfiguration:
                case RemoteActionType.SetConfiguration:
                    if (!device.SupportsRemoteConfiguration)
                        throw new InvalidOperationException("Device does not support remote configuration");
                    break;

                case RemoteActionType.Shutdown:
                    throw new InvalidOperationException("Remote shutdown is disabled for safety");

                default:
                    break;
            }
        }
    }
}
