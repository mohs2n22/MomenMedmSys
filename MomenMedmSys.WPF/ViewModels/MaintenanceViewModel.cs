using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
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
        private readonly IExportService _exportService;
        private readonly Func<MaintenanceFormViewModel> _formFactory;
        private MainViewModel? _mainVM;

        public MaintenanceViewModel(IMaintenanceService maintenanceService, IDeviceService deviceService,
            IDialogService dialogService, IExportService exportService, Func<MaintenanceFormViewModel> formFactory)
        {
            _maintenanceService = maintenanceService;
            _deviceService = deviceService;
            _dialogService = dialogService;
            _exportService = exportService;
            _formFactory = formFactory;
            Title = "Maintenance";
            LoadRecordsCommand.Execute(null);
        }

        public void SetMainViewModel(MainViewModel mainVM) { _mainVM = mainVM; }

        public ObservableCollection<MaintenanceRecord> Records { get; } = new();
        public ObservableCollection<MaintenanceRecord> FilteredRecords { get; } = new();
        public ObservableCollection<MedicalDevice> Devices { get; } = new();

        [ObservableProperty] private MaintenanceRecord? _selectedRecord;
        [ObservableProperty] private int _completedCount;
        [ObservableProperty] private int _overdueCount;
        [ObservableProperty] private int _scheduledCount;
        [ObservableProperty] private string _searchText = string.Empty;
        [ObservableProperty] private string _filterType = "All";
        [ObservableProperty] private string _filterStatus = "All";

        partial void OnSearchTextChanged(string value) => ApplyFilter();
        partial void OnFilterTypeChanged(string value) => ApplyFilter();
        partial void OnFilterStatusChanged(string value) => ApplyFilter();

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
                ScheduledCount = Records.Count(r => r.Status == MaintenanceStatus.Scheduled);

                ApplyFilter();
                StatusMessage = $"Loaded {Records.Count} maintenance records";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading records: {ex.Message}";
            }
            finally { IsLoading = false; }
        }

        private void ApplyFilter()
        {
            FilteredRecords.Clear();
            var query = Records.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var s = SearchText.ToLower();
                query = query.Where(r => r.Title.ToLower().Contains(s) || (r.Device?.DeviceName ?? "").ToLower().Contains(s) ||
                    (r.PerformedBy ?? "").ToLower().Contains(s));
            }

            if (FilterType != "All" && FilterType != "All Types")
            {
                if (Enum.TryParse<MaintenanceType>(FilterType, out var t))
                    query = query.Where(r => r.Type == t);
            }

            if (FilterStatus != "All" && FilterStatus != "All Statuses")
            {
                if (Enum.TryParse<MaintenanceStatus>(FilterStatus, out var st))
                    query = query.Where(r => r.Status == st);
            }

            foreach (var r in query) FilteredRecords.Add(r);
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
        private async Task EditRecord()
        {
            if (SelectedRecord == null)
            {
                await _dialogService.ShowMessageAsync("Please select a record to edit.", "No Selection");
                return;
            }
            var form = _formFactory();
            form.SetEditMode(SelectedRecord);
            _mainVM?.NavigateTo(form);
        }

        [RelayCommand]
        private async Task ExportToExcel()
        {
            var dialog = new SaveFileDialog { Filter = "Excel Files (*.xlsx)|*.xlsx", DefaultExt = ".xlsx", FileName = $"Maintenance_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx", Title = "Export to Excel" };
            if (dialog.ShowDialog() != true) return;
            try
            {
                var headers = new[] { "Title", "Type", "Device", "Scheduled Date", "Status", "Performed By", "Cost", "Downtime (hrs)" };
                var rows = FilteredRecords.Select(r => new object[] {
                    r.Title, r.Type.ToString(), r.DeviceName, r.ScheduledDate, r.Status.ToString(),
                    r.PerformedBy ?? "", r.TotalCost, r.DowntimeHours ?? 0m
                });
                await _exportService.ExportToExcelAsync(dialog.FileName, "Maintenance Records", headers, rows);
                StatusMessage = $"📊 Exported to Excel: {Path.GetFileName(dialog.FileName)}";
                OpenFile(dialog.FileName);
            }
            catch (Exception ex) { StatusMessage = $"❌ Export error: {ex.Message}"; }
        }

        [RelayCommand]
        private async Task ExportToCsv()
        {
            var dialog = new SaveFileDialog { Filter = "CSV Files (*.csv)|*.csv", DefaultExt = ".csv", FileName = $"Maintenance_{DateTime.Now:yyyyMMdd_HHmmss}.csv", Title = "Export to CSV" };
            if (dialog.ShowDialog() != true) return;
            try
            {
                var headers = new[] { "Title", "Type", "Device", "Scheduled Date", "Status", "Performed By", "Cost", "Downtime (hrs)" };
                var rows = FilteredRecords.Select(r => new object[] {
                    r.Title, r.Type.ToString(), r.DeviceName, r.ScheduledDate, r.Status.ToString(),
                    r.PerformedBy ?? "", r.TotalCost, r.DowntimeHours ?? 0m
                });
                await _exportService.ExportToCsvAsync(dialog.FileName, headers, rows);
                StatusMessage = $"📄 Exported to CSV: {Path.GetFileName(dialog.FileName)}";
                OpenFile(dialog.FileName);
            }
            catch (Exception ex) { StatusMessage = $"❌ Export error: {ex.Message}"; }
        }

        [RelayCommand]
        private async Task ExportToPdf()
        {
            var dialog = new SaveFileDialog { Filter = "PDF Files (*.pdf)|*.pdf", DefaultExt = ".pdf", FileName = $"Maintenance_{DateTime.Now:yyyyMMdd_HHmmss}.pdf", Title = "Export to PDF" };
            if (dialog.ShowDialog() != true) return;
            try
            {
                var headers = new[] { "Title", "Type", "Device", "Date", "Status", "Cost" };
                var rows = FilteredRecords.Select(r => new object[] {
                    r.Title, r.Type.ToString(), r.DeviceName, r.ScheduledDate.ToString("yyyy-MM-dd"),
                    r.Status.ToString(), r.TotalCost.ToString("C2")
                });
                var summary = new (string, string)[] {
                    ("Total", FilteredRecords.Count.ToString()),
                    ("Completed", CompletedCount.ToString()),
                    ("Overdue", OverdueCount.ToString()),
                    ("Scheduled", ScheduledCount.ToString())
                };
                await _exportService.ExportToPdfAsync(dialog.FileName, "Maintenance Records Report",
                    $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", headers, rows, summary);
                StatusMessage = $"📑 Exported to PDF: {Path.GetFileName(dialog.FileName)}";
                OpenFile(dialog.FileName);
            }
            catch (Exception ex) { StatusMessage = $"❌ Export error: {ex.Message}"; }
        }

        private async void OpenFile(string path)
        {
            if (await _dialogService.ShowConfirmAsync($"File saved:\n{path}\n\nOpen file?", "Open File"))
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = path, UseShellExecute = true });
        }
    }
}
