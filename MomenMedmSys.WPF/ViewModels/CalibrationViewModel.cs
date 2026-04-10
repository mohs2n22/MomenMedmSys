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
    public partial class CalibrationViewModel : ViewModelBase
    {
        private readonly ICalibrationService _calibrationService;
        private readonly IDeviceService _deviceService;
        private readonly IDialogService _dialogService;
        private readonly Func<CalibrationFormViewModel> _formFactory;
        private MainViewModel? _mainVM;

        public CalibrationViewModel(ICalibrationService calibrationService, IDeviceService deviceService,
            IDialogService dialogService, Func<CalibrationFormViewModel> formFactory)
        {
            _calibrationService = calibrationService;
            _deviceService = deviceService;
            _dialogService = dialogService;
            _formFactory = formFactory;
            Title = "Calibration";
            LoadRecordsCommand.Execute(null);
        }

        public void SetMainViewModel(MainViewModel mainVM) { _mainVM = mainVM; }

        public ObservableCollection<CalibrationRecord> Records { get; } = new();
        public ObservableCollection<MedicalDevice> Devices { get; } = new();

        [ObservableProperty] private CalibrationRecord? _selectedRecord;
        [ObservableProperty] private int _overdueCount;
        [ObservableProperty] private int _passCount;
        [ObservableProperty] private int _failCount;

        [RelayCommand]
        private async Task LoadRecords()
        {
            IsLoading = true;
            try
            {
                Records.Clear();
                var all = await _calibrationService.GetAllRecordsAsync();
                foreach (var r in all) Records.Add(r);

                Devices.Clear();
                var devices = await _deviceService.GetAllDevicesAsync();
                foreach (var d in devices) Devices.Add(d);

                OverdueCount = await _calibrationService.GetOverdueCountAsync();
                PassCount = await _calibrationService.GetPassCountAsync();
                FailCount = await _calibrationService.GetFailCountAsync();

                StatusMessage = $"Loaded {Records.Count} calibration records";
            }
            catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
            finally { IsLoading = false; }
        }

        [RelayCommand]
        private async Task DeleteRecord()
        {
            if (SelectedRecord == null) return;
            var confirmed = await _dialogService.ShowConfirmAsync("Delete this calibration record?", "Confirm");
            if (confirmed)
            {
                await _calibrationService.DeleteRecordAsync(SelectedRecord.Id);
                Records.Remove(SelectedRecord);
                SelectedRecord = null;
                StatusMessage = "Record deleted";
            }
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
            if (SelectedRecord == null) { _dialogService.ShowMessageAsync("Select a record to edit.", "No Selection"); return; }
            var form = _formFactory();
            form.SetEditMode(SelectedRecord);
            _mainVM?.NavigateTo(form);
        }
    }
}
