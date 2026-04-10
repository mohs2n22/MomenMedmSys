using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Services;
using MomenMedmSys.WPF.Services;
using MomenMedmSys.WPF.ViewModels.Base;

namespace MomenMedmSys.WPF.ViewModels
{
    public partial class NetworkDevicesViewModel : ViewModelBase
    {
        private readonly INetworkDiscoveryService _networkService;
        private readonly IDialogService _dialogService;

        public NetworkDevicesViewModel(INetworkDiscoveryService networkService, IDialogService dialogService)
        {
            _networkService = networkService;
            _dialogService = dialogService;
            Title = "Network Devices";
            LoadDevicesCommand.Execute(null);
        }

        public ObservableCollection<NetworkDevice> Devices { get; } = new();
        public ObservableCollection<NetworkDevice> DiscoveredDevices { get; } = new();
        public ObservableCollection<DeviceActionLog> ActionLogs { get; } = new();

        [ObservableProperty] private NetworkDevice? _selectedDevice;
        [ObservableProperty] private NetworkDevice? _discoveredDeviceToImport;

        // Status counts
        [ObservableProperty] private int _onlineCount;
        [ObservableProperty] private int _offlineCount;
        [ObservableProperty] private int _warningCount;
        [ObservableProperty] private int _totalCount;

        // Discovery
        [ObservableProperty] private string _subnetInput = string.Empty;
        [ObservableProperty] private bool _isScanning;
        [ObservableProperty] private int _scanProgress;

        // Detail tabs
        [ObservableProperty] private int _detailTabIndex;

        // Remote action
        [ObservableProperty] private string _actionParameters = string.Empty;
        [ObservableProperty] private RemoteActionType _selectedActionType;
        [ObservableProperty] private string _actionResultMessage = string.Empty;

        [RelayCommand]
        private async Task LoadDevices()
        {
            IsLoading = true;
            try
            {
                Devices.Clear();
                var all = await _networkService.GetAllNetworkDevicesAsync();
                foreach (var d in all) Devices.Add(d);

                OnlineCount = await _networkService.GetOnlineCountAsync();
                OfflineCount = await _networkService.GetOfflineCountAsync();
                WarningCount = await _networkService.GetWarningCountAsync();
                TotalCount = Devices.Count;

                StatusMessage = $"Loaded {TotalCount} network devices";
            }
            catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
            finally { IsLoading = false; }
        }

        [RelayCommand]
        private async Task RefreshStatus()
        {
            IsLoading = true;
            try
            {
                await _networkService.RefreshAllDeviceStatusesAsync();
                await LoadDevicesCommand.ExecuteAsync(null);
                StatusMessage = "All device statuses refreshed";
            }
            catch (Exception ex) { StatusMessage = $"Error refreshing: {ex.Message}"; }
            finally { IsLoading = false; }
        }

        [RelayCommand]
        private async Task ScanNetwork()
        {
            IsScanning = true;
            ScanProgress = 0;
            StatusMessage = "Scanning network...";
            try
            {
                DiscoveredDevices.Clear();
                var discovered = await _networkService.DiscoverNetworkAsync(
                    string.IsNullOrWhiteSpace(SubnetInput) ? "" : SubnetInput);

                foreach (var d in discovered)
                    DiscoveredDevices.Add(d);

                ScanProgress = 100;
                StatusMessage = $"Scan complete. Found {discovered.Count} new devices.";
            }
            catch (Exception ex) { StatusMessage = $"Scan error: {ex.Message}"; }
            finally { IsScanning = false; }
        }

        [RelayCommand]
        private async Task ImportSelectedDevice()
        {
            if (DiscoveredDeviceToImport == null) return;

            try
            {
                var device = new NetworkDevice
                {
                    IpAddress = DiscoveredDeviceToImport.IpAddress,
                    MacAddress = DiscoveredDeviceToImport.MacAddress,
                    DeviceName = DiscoveredDeviceToImport.IpAddress,
                    ConnectionStatus = DeviceConnectionStatus.Online,
                    ResponseTimeMs = DiscoveredDeviceToImport.ResponseTimeMs,
                    DiscoveredVia = DiscoveryMethod.NetworkScan,
                    FirstDiscovered = DateTime.Now,
                    LastSeen = DateTime.Now
                };

                await _networkService.AddDeviceAsync(device);
                Devices.Add(device);
                DiscoveredDevices.Remove(DiscoveredDeviceToImport);
                DiscoveredDeviceToImport = null;
                TotalCount = Devices.Count;

                StatusMessage = "Device added to network registry";
            }
            catch (Exception ex) { StatusMessage = $"Import error: {ex.Message}"; }
        }

        [RelayCommand]
        private async Task DeleteDevice()
        {
            if (SelectedDevice == null) return;
            var confirmed = await _dialogService.ShowConfirmAsync($"Delete network device '{SelectedDevice.DeviceName}'?", "Confirm");
            if (confirmed)
            {
                await _networkService.DeleteDeviceAsync(SelectedDevice.Id);
                Devices.Remove(SelectedDevice);
                SelectedDevice = null;
                TotalCount = Devices.Count;
                ActionLogs.Clear();
                StatusMessage = "Device deleted";
            }
        }

        [RelayCommand]
        private async Task ExecuteAction()
        {
            if (SelectedDevice == null)
            {
                await _dialogService.ShowMessageAsync("Please select a device first.", "No Selection");
                return;
            }

            if (!SelectedDevice.IsOnline)
            {
                await _dialogService.ShowMessageAsync("Device is offline. Cannot execute remote actions.", "Device Offline");
                return;
            }

            IsLoading = true;
            ActionResultMessage = string.Empty;
            try
            {
                var log = await _networkService.ExecuteRemoteActionAsync(
                    SelectedDevice.Id,
                    SelectedActionType,
                    ActionParameters,
                    "Administrator");

                ActionLogs.Clear();
                var logs = await _networkService.GetActionLogsAsync(SelectedDevice.Id);
                foreach (var l in logs.OrderByDescending(x => x.CreatedAt))
                    ActionLogs.Add(l);

                ActionResultMessage = log.Result == DeviceActionResult.Success
                    ? $"Action '{log.ActionType}' completed successfully"
                    : $"Action failed: {log.ResultMessage}";

                StatusMessage = ActionResultMessage;
            }
            catch (Exception ex) { ActionResultMessage = $"Error: {ex.Message}"; }
            finally { IsLoading = false; }
        }

        [RelayCommand]
        private async Task LoadActionLogs()
        {
            if (SelectedDevice == null) return;
            ActionLogs.Clear();
            var logs = await _networkService.GetActionLogsAsync(SelectedDevice.Id);
            foreach (var log in logs.OrderByDescending(x => x.CreatedAt))
                ActionLogs.Add(log);
        }

        [RelayCommand]
        private void SelectDevice(NetworkDevice device)
        {
            SelectedDevice = device;
            ActionLogs.Clear();
            LoadActionLogsCommand.Execute(null);
        }
    }
}
