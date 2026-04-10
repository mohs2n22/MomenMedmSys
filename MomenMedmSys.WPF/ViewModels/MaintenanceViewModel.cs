using System;
using System.Collections.ObjectModel;
using System.Linq;
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
    public partial class MaintenanceViewModel : ViewModelBase
    {
        private readonly IMaintenanceService _maintenanceService;
        private readonly IDeviceService _deviceService;
        private readonly IDialogService _dialogService;
        private readonly Func<MaintenanceFormViewModel> _formFactory;
        private MainViewModel? _mainVM;

        public MaintenanceViewModel(IMaintenanceService maintenanceService, IDeviceService deviceService,
            IDialogService dialogService, Func<MaintenanceFormViewModel> formFactory)
        {
            _maintenanceService = maintenanceService;
            _deviceService = deviceService;
            _dialogService = dialogService;
            _formFactory = formFactory;
            Title = "Maintenance";
            LoadRecordsCommand.Execute(null);
        }

        public void SetMainViewModel(MainViewModel mainVM) { _mainVM = mainVM; }

        public ObservableCollection<MaintenanceRecord> Records { get; } = new();
        public ObservableCollection<MedicalDevice> Devices { get; } = new();

        [ObservableProperty] private MaintenanceRecord? _selectedRecord;
        [ObservableProperty] private int _completedCount;
        [ObservableProperty] private int _overdueCount;
        [ObservableProperty] private string _filterType = "All";
        [ObservableProperty] private string _filterStatus = "All";

        [RelayCommand]
        private async Task LoadRecords()
        {
            IsLoading = true;
            try
            {
                Records.Clear();
                var allRecords = await _maintenanceService.GetAllRecordsAsync();
                foreach (var r in allRecords) Records.Add(r);

                // Load devices for dropdown
                Devices.Clear();
                var devices = await _deviceService.GetAllDevicesAsync();
                foreach (var d in devices) Devices.Add(d);

                var now = DateTime.Now;
                CompletedCount = Records.Count(r => r.Status == MaintenanceStatus.Completed);
                OverdueCount = Records.Count(r => r.Status == MaintenanceStatus.Scheduled && r.ScheduledDate < now);

                StatusMessage = $"Loaded {Records.Count} maintenance records";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading records: {ex.Message}";
            }
            finally { IsLoading = false; }
        }

        [RelayCommand]
        private async Task DeleteRecord()
        {
            if (SelectedRecord == null) return;
            var confirmed = await _dialogService.ShowConfirmAsync($"Delete maintenance record '{SelectedRecord.Title}'?", "Confirm");
            if (confirmed)
            {
                await _maintenanceService.DeleteRecordAsync(SelectedRecord.Id);
                Records.Remove(SelectedRecord);
                SelectedRecord = null;
                StatusMessage = "Record deleted";
            }
        }

        [RelayCommand]
        private async Task Refresh()
        {
            await LoadRecordsCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        private void AddRecord()
        {
            var form = _formFactory();
            form.SetAddMode(SelectedRecord?.DeviceId);
            _mainVM?.NavigateTo(form);
        }

        [RelayCommand]
        private void EditRecord()
        {
            if (SelectedRecord == null)
            {
                _dialogService.ShowMessageAsync("Please select a record to edit.", "No Selection");
                return;
            }
            var form = _formFactory();
            form.SetEditMode(SelectedRecord);
            _mainVM?.NavigateTo(form);
        }
    }
}
