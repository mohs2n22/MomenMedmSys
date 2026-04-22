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
    public partial class CalibrationViewModel : ViewModelBase
    {
        private readonly ICalibrationService _calibrationService;
        private readonly IDeviceService _deviceService;
        private readonly IDialogService _dialogService;
        private readonly IExportService _exportService;
        private readonly Func<CalibrationFormViewModel> _formFactory;
        private MainViewModel? _mainVM;

        public CalibrationViewModel(ICalibrationService calibrationService, IDeviceService deviceService,
            IDialogService dialogService, IExportService exportService, Func<CalibrationFormViewModel> formFactory)
        {
            _calibrationService = calibrationService;
            _deviceService = deviceService;
            _dialogService = dialogService;
            _exportService = exportService;
            _formFactory = formFactory;
            Title = "Calibration";
            LoadRecordsCommand.Execute(null);
        }

        public void SetMainViewModel(MainViewModel mainVM) { _mainVM = mainVM; }

        public ObservableCollection<CalibrationRecord> Records { get; } = new();
        public ObservableCollection<CalibrationRecord> FilteredRecords { get; } = new();
        public ObservableCollection<MedicalDevice> Devices { get; } = new();

        [ObservableProperty] private CalibrationRecord? _selectedRecord;
        [ObservableProperty] private int _overdueCount;
        [ObservableProperty] private int _passCount;
        [ObservableProperty] private int _failCount;
        [ObservableProperty] private string _searchText = string.Empty;
        [ObservableProperty] private string _filterResult = "All";

        partial void OnSearchTextChanged(string value) => ApplyFilter();
        partial void OnFilterResultChanged(string value) => ApplyFilter();

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

                ApplyFilter();
                StatusMessage = $"Loaded {Records.Count} calibration records";
            }
            catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
            finally { IsLoading = false; }
        }

        private void ApplyFilter()
        {
            FilteredRecords.Clear();
            var query = Records.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var s = SearchText.ToLower();
                query = query.Where(r => r.CalibrationType.ToLower().Contains(s) || (r.Device?.DeviceName ?? "").ToLower().Contains(s) ||
                    (r.PerformedBy ?? "").ToLower().Contains(s) || (r.CertificateNumber ?? "").ToLower().Contains(s));
            }

            if (FilterResult != "All" && FilterResult != "All Results")
            {
                if (Enum.TryParse<CalibrationResult>(FilterResult, out var res))
                    query = query.Where(r => r.Result == res);
            }

            foreach (var r in query) FilteredRecords.Add(r);
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
        private async Task EditRecord()
        {
            if (SelectedRecord == null) { await _dialogService.ShowMessageAsync("Select a record to edit.", "No Selection"); return; }
            var form = _formFactory();
            form.SetEditMode(SelectedRecord);
            _mainVM?.NavigateTo(form);
        }

        [RelayCommand]
        private async Task ExportToExcel()
        {
            var dialog = new SaveFileDialog { Filter = "Excel Files (*.xlsx)|*.xlsx", DefaultExt = ".xlsx", FileName = $"Calibration_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx", Title = "Export to Excel" };
            if (dialog.ShowDialog() != true) return;
            try
            {
                var headers = new[] { "Type", "Device", "Date", "Next Due", "Result", "Performed By", "Lab", "Certificate#" };
                var rows = FilteredRecords.Select(r => new object[] {
                    r.CalibrationType, r.DeviceName, r.CalibrationDate, r.NextDueDate, r.Result.ToString(),
                    r.PerformedBy, r.IsExternalLab ? "External" : "Internal", r.CertificateNumber
                });
                await _exportService.ExportToExcelAsync(dialog.FileName, "Calibration Records", headers, rows);
                StatusMessage = $"📊 Exported to Excel: {Path.GetFileName(dialog.FileName)}";
                OpenFile(dialog.FileName);
            }
            catch (Exception ex) { StatusMessage = $"❌ Export error: {ex.Message}"; }
        }

        [RelayCommand]
        private async Task ExportToCsv()
        {
            var dialog = new SaveFileDialog { Filter = "CSV Files (*.csv)|*.csv", DefaultExt = ".csv", FileName = $"Calibration_{DateTime.Now:yyyyMMdd_HHmmss}.csv", Title = "Export to CSV" };
            if (dialog.ShowDialog() != true) return;
            try
            {
                var headers = new[] { "Type", "Device", "Date", "Next Due", "Result", "Performed By", "Lab", "Certificate#" };
                var rows = FilteredRecords.Select(r => new object[] {
                    r.CalibrationType, r.DeviceName, r.CalibrationDate, r.NextDueDate, r.Result.ToString(),
                    r.PerformedBy, r.IsExternalLab ? "External" : "Internal", r.CertificateNumber
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
            var dialog = new SaveFileDialog { Filter = "PDF Files (*.pdf)|*.pdf", DefaultExt = ".pdf", FileName = $"Calibration_{DateTime.Now:yyyyMMdd_HHmmss}.pdf", Title = "Export to PDF" };
            if (dialog.ShowDialog() != true) return;
            try
            {
                var headers = new[] { "Type", "Device", "Date", "Next Due", "Result", "Lab" };
                var rows = FilteredRecords.Select(r => new object[] {
                    r.CalibrationType, r.DeviceName, r.CalibrationDate.ToString("yyyy-MM-dd"),
                    r.NextDueDate.ToString("yyyy-MM-dd"), r.Result.ToString(), r.IsExternalLab ? "External" : "Internal"
                });
                var summary = new (string, string)[] {
                    ("Total", FilteredRecords.Count.ToString()),
                    ("Pass", PassCount.ToString()),
                    ("Fail", FailCount.ToString()),
                    ("Overdue", OverdueCount.ToString())
                };
                await _exportService.ExportToPdfAsync(dialog.FileName, "Calibration Records Report",
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
