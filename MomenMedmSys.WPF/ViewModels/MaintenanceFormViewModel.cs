using System;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Services;
using MomenMedmSys.WPF.Services;

using MomenMedmSys.WPF.ViewModels.Base;

namespace MomenMedmSys.WPF.ViewModels
{
    public partial class MaintenanceFormViewModel : ViewModelBase
    {
        private readonly IMaintenanceService _maintenanceService;
        private readonly IDialogService _dialogService;

        public string Mode { get; private set; } = "Add";
        public MaintenanceRecord? EditingRecord { get; private set; }

        public MaintenanceFormViewModel(IMaintenanceService maintenanceService, IDialogService dialogService)
        {
            _maintenanceService = maintenanceService;
            _dialogService = dialogService;
            Title = "Add Maintenance Record";
        }

        public void SetEditMode(MaintenanceRecord record)
        {
            Mode = "Edit";
            EditingRecord = record;
            Title = "Edit Maintenance Record";

            DeviceId = record.DeviceId;
            Type = record.Type;
            RecordTitle = record.Title;
            Description = record.Description;
            ScheduledDate = record.ScheduledDate;
            CompletedDate = record.CompletedDate;
            PerformedBy = record.PerformedBy;
            Status = record.Status;
            Findings = record.Findings;
            ActionsTaken = record.ActionsTaken;
            LaborCost = record.LaborCost;
            PartsCost = record.PartsCost;
            Recurrence = record.Recurrence;
            RecurrenceInterval = record.RecurrenceInterval;
            Recommendations = record.Recommendations;
            StatusMessage = $"Editing: {record.Title}";
        }

        public void SetAddMode(int? defaultDeviceId = null)
        {
            Mode = "Add";
            EditingRecord = null;
            Title = "Add Maintenance Record";
            DeviceId = defaultDeviceId ?? 0;
            Type = MaintenanceType.Preventive;
            RecordTitle = string.Empty;
            Description = string.Empty;
            ScheduledDate = DateTime.Now.AddDays(7);
            CompletedDate = null;
            PerformedBy = string.Empty;
            Status = MaintenanceStatus.Scheduled;
            Findings = string.Empty;
            ActionsTaken = string.Empty;
            LaborCost = 0;
            PartsCost = 0;
            Recurrence = RecurrenceFrequency.None;
            RecurrenceInterval = 0;
            Recommendations = string.Empty;
            StatusMessage = "Fill in maintenance details";
        }

        [ObservableProperty] private int _deviceId;
        [ObservableProperty] private MaintenanceType _type = MaintenanceType.Preventive;
        [ObservableProperty] private string _recordTitle = string.Empty;
        [ObservableProperty] private string _description = string.Empty;
        [ObservableProperty] private DateTime _scheduledDate = DateTime.Now.AddDays(7);
        [ObservableProperty] private DateTime? _completedDate;
        [ObservableProperty] private string _performedBy = string.Empty;
        [ObservableProperty] private MaintenanceStatus _status = MaintenanceStatus.Scheduled;
        [ObservableProperty] private string _findings = string.Empty;
        [ObservableProperty] private string _actionsTaken = string.Empty;
        [ObservableProperty] private decimal _laborCost;
        [ObservableProperty] private decimal _partsCost;
        [ObservableProperty] private RecurrenceFrequency _recurrence = RecurrenceFrequency.None;
        [ObservableProperty] private int _recurrenceInterval;
        [ObservableProperty] private string _recommendations = string.Empty;

        public MaintenanceType[] TypeOptions => (MaintenanceType[])Enum.GetValues(typeof(MaintenanceType));
        public MaintenanceStatus[] StatusOptions => (MaintenanceStatus[])Enum.GetValues(typeof(MaintenanceStatus));
        public RecurrenceFrequency[] RecurrenceOptions => (RecurrenceFrequency[])Enum.GetValues(typeof(RecurrenceFrequency));

        [RelayCommand]
        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(RecordTitle))
            {
                await _dialogService.ShowMessageAsync("Title is required.", "Validation Error");
                return;
            }

            try
            {
                if (Mode == "Edit" && EditingRecord != null)
                {
                    EditingRecord.Type = Type;
                    EditingRecord.Title = RecordTitle;
                    EditingRecord.Description = Description;
                    EditingRecord.ScheduledDate = ScheduledDate;
                    EditingRecord.CompletedDate = CompletedDate;
                    EditingRecord.PerformedBy = PerformedBy;
                    EditingRecord.Status = Status;
                    EditingRecord.Findings = Findings;
                    EditingRecord.ActionsTaken = ActionsTaken;
                    EditingRecord.LaborCost = LaborCost;
                    EditingRecord.PartsCost = PartsCost;
                    EditingRecord.Recurrence = Recurrence;
                    EditingRecord.RecurrenceInterval = RecurrenceInterval;
                    EditingRecord.Recommendations = Recommendations;
                    EditingRecord.UpdatedAt = DateTime.Now;

                    await _maintenanceService.UpdateRecordAsync(EditingRecord);
                    StatusMessage = $"Updated: {RecordTitle}";
                }
                else
                {
                    var record = new MaintenanceRecord
                    {
                        DeviceId = DeviceId,
                        Type = Type,
                        Title = RecordTitle,
                        Description = Description,
                        ScheduledDate = ScheduledDate,
                        CompletedDate = CompletedDate,
                        PerformedBy = PerformedBy,
                        Status = Status,
                        Findings = Findings,
                        ActionsTaken = ActionsTaken,
                        LaborCost = LaborCost,
                        PartsCost = PartsCost,
                        Recurrence = Recurrence,
                        RecurrenceInterval = RecurrenceInterval,
                        Recommendations = Recommendations,
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    };

                    await _maintenanceService.CreateRecordAsync(record);
                    StatusMessage = $"Created: {Title}";
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
