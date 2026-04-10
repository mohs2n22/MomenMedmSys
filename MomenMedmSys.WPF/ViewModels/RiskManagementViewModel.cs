using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Services;
using MomenMedmSys.WPF.Services;
using MomenMedmSys.WPF.ViewModels;
using MomenMedmSys.WPF.ViewModels.Base;

namespace MomenMedmSys.WPF.ViewModels
{
    public partial class RiskManagementViewModel : ViewModelBase
    {
        private readonly IRiskService _riskService;
        private readonly IDeviceService _deviceService;
        private readonly IDialogService _dialogService;
        private MainViewModel? _mainVM;

        public RiskManagementViewModel(IRiskService riskService, IDeviceService deviceService, IDialogService dialogService)
        {
            _riskService = riskService;
            _deviceService = deviceService;
            _dialogService = dialogService;
            Title = "Risk Management";
            LoadIncidentsCommand.Execute(null);
        }

        public void SetMainViewModel(MainViewModel mainVM) { _mainVM = mainVM; }

        public ObservableCollection<RiskIncident> Incidents { get; } = new();
        public ObservableCollection<MedicalDevice> Devices { get; } = new();

        [ObservableProperty] private RiskIncident? _selectedIncident;
        [ObservableProperty] private int _openIncidentsCount;
        [ObservableProperty] private int _criticalIncidentsCount;

        [RelayCommand]
        private async Task LoadIncidents()
        {
            IsLoading = true;
            try
            {
                Incidents.Clear();
                var all = await _riskService.GetAllIncidentsAsync();
                foreach (var i in all) Incidents.Add(i);

                Devices.Clear();
                var devices = await _deviceService.GetAllDevicesAsync();
                foreach (var d in devices) Devices.Add(d);

                OpenIncidentsCount = await _riskService.GetOpenIncidentCountAsync();
                CriticalIncidentsCount = await _riskService.GetCriticalIncidentCountAsync();

                StatusMessage = $"Loaded {Incidents.Count} incidents";
            }
            catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
            finally { IsLoading = false; }
        }

        [RelayCommand]
        private async Task DeleteIncident()
        {
            if (SelectedIncident == null) return;
            var confirmed = await _dialogService.ShowConfirmAsync("Delete this incident?", "Confirm");
            if (confirmed)
            {
                await _riskService.DeleteIncidentAsync(SelectedIncident.Id);
                Incidents.Remove(SelectedIncident);
                SelectedIncident = null;
                StatusMessage = "Incident deleted";
            }
        }
    }
}
