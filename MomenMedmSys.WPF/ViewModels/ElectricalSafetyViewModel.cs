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
    public partial class ElectricalSafetyViewModel : ViewModelBase
    {
        private readonly IElectricalSafetyService _safetyService;
        private readonly IDeviceService _deviceService;
        private readonly IDialogService _dialogService;
        private MainViewModel? _mainVM;

        public ElectricalSafetyViewModel(IElectricalSafetyService safetyService, IDeviceService deviceService, IDialogService dialogService)
        {
            _safetyService = safetyService;
            _deviceService = deviceService;
            _dialogService = dialogService;
            Title = "Electrical Safety Tests";
            LoadTestsCommand.Execute(null);
        }

        public void SetMainViewModel(MainViewModel mainVM) { _mainVM = mainVM; }

        public ObservableCollection<ElectricalSafetyTest> Tests { get; } = new();
        public ObservableCollection<MedicalDevice> Devices { get; } = new();

        [ObservableProperty] private ElectricalSafetyTest? _selectedTest;
        [ObservableProperty] private int _overdueCount;
        [ObservableProperty] private int _passCount;
        [ObservableProperty] private int _failCount;

        [RelayCommand]
        private async Task LoadTests()
        {
            IsLoading = true;
            try
            {
                Tests.Clear();
                var all = await _safetyService.GetAllTestsAsync();
                foreach (var t in all) Tests.Add(t);

                Devices.Clear();
                var devices = await _deviceService.GetAllDevicesAsync();
                foreach (var d in devices) Devices.Add(d);

                OverdueCount = await _safetyService.GetOverdueCountAsync();
                PassCount = await _safetyService.GetPassCountAsync();
                FailCount = await _safetyService.GetFailCountAsync();

                StatusMessage = $"Loaded {Tests.Count} safety tests";
            }
            catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
            finally { IsLoading = false; }
        }

        [RelayCommand]
        private async Task DeleteTest()
        {
            if (SelectedTest == null) return;
            var confirmed = await _dialogService.ShowConfirmAsync("Delete this safety test?", "Confirm");
            if (confirmed)
            {
                await _safetyService.DeleteTestAsync(SelectedTest.Id);
                Tests.Remove(SelectedTest);
                SelectedTest = null;
                StatusMessage = "Test deleted";
            }
        }
    }
}
