using System;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Services;
using MomenMedmSys.WPF.Services;
using MomenMedmSys.WPF.ViewModels;
using MomenMedmSys.WPF.ViewModels.Base;

namespace MomenMedmSys.WPF.ViewModels
{
    public partial class CalibrationFormViewModel : ViewModelBase
    {
        private readonly ICalibrationService _calibrationService;
        private readonly IDialogService _dialogService;
        

        public string Mode { get; private set; } = "Add";
        public CalibrationRecord? EditingRecord { get; private set; }

        public CalibrationFormViewModel(ICalibrationService calibrationService, IDialogService dialogService)
        {
            _calibrationService = calibrationService;
            _dialogService = dialogService;
            Title = "Add Calibration Record";
        }

        public void SetEditMode(CalibrationRecord record)
        {
            Mode = "Edit";
            EditingRecord = record;
            Title = "Edit Calibration Record";
            DeviceId = record.DeviceId;
            CalibrationType = record.CalibrationType;
            StandardUsed = record.StandardUsed;
            CalibrationDate = record.CalibrationDate;
            NextDueDate = record.NextDueDate;
            PerformedBy = record.PerformedBy;
            IsExternalLab = record.IsExternalLab;
            LaboratoryName = record.LaboratoryName;
            Result = record.Result;
            AsFoundData = record.AsFoundData;
            AsLeftData = record.AsLeftData;
            CertificateNumber = record.CertificateNumber;
            Remarks = record.Remarks;
            StatusMessage = $"Editing: {record.CalibrationType}";
        }

        public void SetAddMode(int? defaultDeviceId = null)
        {
            Mode = "Add";
            EditingRecord = null;
            Title = "Add Calibration Record";
            DeviceId = defaultDeviceId ?? 0;
            CalibrationType = "Full Calibration";
            StandardUsed = string.Empty;
            CalibrationDate = DateTime.Now;
            NextDueDate = DateTime.Now.AddMonths(6);
            PerformedBy = string.Empty;
            IsExternalLab = false;
            LaboratoryName = string.Empty;
            Result = CalibrationResult.Pass;
            AsFoundData = string.Empty;
            AsLeftData = string.Empty;
            CertificateNumber = string.Empty;
            Remarks = string.Empty;
            StatusMessage = "Fill in calibration details";
        }

        [ObservableProperty] private int _deviceId;
        [ObservableProperty] private string _calibrationType = "Full Calibration";
        [ObservableProperty] private string _standardUsed = string.Empty;
        [ObservableProperty] private DateTime _calibrationDate = DateTime.Now;
        [ObservableProperty] private DateTime _nextDueDate = DateTime.Now.AddMonths(6);
        [ObservableProperty] private string _performedBy = string.Empty;
        [ObservableProperty] private bool _isExternalLab;
        [ObservableProperty] private string _laboratoryName = string.Empty;
        [ObservableProperty] private CalibrationResult _result = CalibrationResult.Pass;
        [ObservableProperty] private string _asFoundData = string.Empty;
        [ObservableProperty] private string _asLeftData = string.Empty;
        [ObservableProperty] private string _certificateNumber = string.Empty;
        [ObservableProperty] private string _remarks = string.Empty;

        [RelayCommand]
        private async Task Save()
        {
            if (DeviceId == 0)
            {
                await _dialogService.ShowMessageAsync("Please select a device.", "Validation Error");
                return;
            }

            try
            {
                if (Mode == "Edit" && EditingRecord != null)
                {
                    EditingRecord.CalibrationType = CalibrationType;
                    EditingRecord.StandardUsed = StandardUsed;
                    EditingRecord.CalibrationDate = CalibrationDate;
                    EditingRecord.NextDueDate = NextDueDate;
                    EditingRecord.PerformedBy = PerformedBy;
                    EditingRecord.IsExternalLab = IsExternalLab;
                    EditingRecord.LaboratoryName = LaboratoryName;
                    EditingRecord.Result = Result;
                    EditingRecord.AsFoundData = AsFoundData;
                    EditingRecord.AsLeftData = AsLeftData;
                    EditingRecord.CertificateNumber = CertificateNumber;
                    EditingRecord.Remarks = Remarks;
                    EditingRecord.UpdatedAt = DateTime.Now;

                    await _calibrationService.UpdateRecordAsync(EditingRecord);
                    StatusMessage = $"Updated: {CalibrationType}";
                }
                else
                {
                    var record = new CalibrationRecord
                    {
                        DeviceId = DeviceId,
                        CalibrationType = CalibrationType,
                        StandardUsed = StandardUsed,
                        CalibrationDate = CalibrationDate,
                        NextDueDate = NextDueDate,
                        PerformedBy = PerformedBy,
                        IsExternalLab = IsExternalLab,
                        LaboratoryName = LaboratoryName,
                        Result = Result,
                        AsFoundData = AsFoundData,
                        AsLeftData = AsLeftData,
                        CertificateNumber = CertificateNumber,
                        Remarks = Remarks,
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    };

                    await _calibrationService.CreateRecordAsync(record);
                    StatusMessage = $"Created: {CalibrationType}";
                }

                App.MainViewModelInstance?.GoBackCommand.Execute(null);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Save error: {ex.Message}";
                await _dialogService.ShowMessageAsync($"Failed to save: {ex.Message}", "Error");
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            StatusMessage = "Form cancelled";
            App.MainViewModelInstance?.GoBackCommand.Execute(null);
        }
    }
}
