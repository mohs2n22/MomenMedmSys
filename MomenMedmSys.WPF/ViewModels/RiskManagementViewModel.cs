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
    public partial class RiskManagementViewModel : ViewModelBase
    {
        private readonly IRiskService _riskService;
        private readonly IDeviceService _deviceService;
        private readonly IDialogService _dialogService;
        private readonly IExportService _exportService;
        private readonly Func<RiskIncidentFormViewModel>? _formFactory;
        private MainViewModel? _mainVM;

        public RiskManagementViewModel(IRiskService riskService, IDeviceService deviceService, IDialogService dialogService, IExportService exportService, Func<RiskIncidentFormViewModel>? formFactory = null)
        {
            _riskService = riskService;
            _deviceService = deviceService;
            _dialogService = dialogService;
            _exportService = exportService;
            _formFactory = formFactory;
            Title = "Risk Management";
            LoadIncidentsCommand.Execute(null);
        }

        public void SetMainViewModel(MainViewModel mainVM) { _mainVM = mainVM; }

        public ObservableCollection<RiskIncident> Incidents { get; } = new();
        public ObservableCollection<RiskIncident> FilteredIncidents { get; } = new();
        public ObservableCollection<MedicalDevice> Devices { get; } = new();

        [ObservableProperty] private RiskIncident? _selectedIncident;
        [ObservableProperty] private int _openIncidentsCount;
        [ObservableProperty] private int _criticalIncidentsCount;
        [ObservableProperty] private int _highRiskCount;
        [ObservableProperty] private int _closedCount;
        [ObservableProperty] private string _searchText = string.Empty;
        [ObservableProperty] private string _filterRiskLevel = "All";
        [ObservableProperty] private string _filterStatus = "All";

        partial void OnSearchTextChanged(string value) => ApplyFilter();
        partial void OnFilterRiskLevelChanged(string value) => ApplyFilter();
        partial void OnFilterStatusChanged(string value) => ApplyFilter();

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
                HighRiskCount = Incidents.Count(i => i.OverallRisk == RiskLevel.High);
                ClosedCount = Incidents.Count(i => i.Status == IncidentStatus.Closed);

                ApplyFilter();
                StatusMessage = $"Loaded {Incidents.Count} incidents";
            }
            catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
            finally { IsLoading = false; }
        }

        private void ApplyFilter()
        {
            FilteredIncidents.Clear();
            var query = Incidents.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var s = SearchText.ToLower();
                query = query.Where(i => i.Title.ToLower().Contains(s) || i.IncidentCode.ToLower().Contains(s) ||
                    (i.Device?.DeviceName ?? "").ToLower().Contains(s) || i.Description.ToLower().Contains(s));
            }

            if (FilterRiskLevel != "All" && FilterRiskLevel != "All Levels")
            {
                if (Enum.TryParse<RiskLevel>(FilterRiskLevel, out var rl))
                    query = query.Where(i => i.OverallRisk == rl);
            }

            if (FilterStatus != "All" && FilterStatus != "All Statuses")
            {
                if (Enum.TryParse<IncidentStatus>(FilterStatus, out var st))
                    query = query.Where(i => i.Status == st);
            }

            foreach (var i in query) FilteredIncidents.Add(i);
        }

        [RelayCommand]
        private void AddIncident()
        {
            if (_formFactory == null || _mainVM == null) return;
            var form = _formFactory();
            form.SetAddMode();
            _mainVM.NavigateTo(form);
        }

        [RelayCommand]
        private async Task EditIncident(RiskIncident? incident)
        {
            if (_formFactory == null || _mainVM == null) return;
            var target = incident ?? SelectedIncident;
            if (target == null) { await _dialogService.ShowMessageAsync("Select an incident to edit.", "No Selection"); return; }
            var form = _formFactory();
            form.SetEditMode(target);
            _mainVM.NavigateTo(form);
        }

        [RelayCommand]
        private async Task DeleteIncident(RiskIncident? incident)
        {
            var target = incident ?? SelectedIncident;
            if (target == null) return;
            var confirmed = await _dialogService.ShowConfirmAsync("Delete this incident?", "Confirm");
            if (confirmed)
            {
                await _riskService.DeleteIncidentAsync(target.Id);
                Incidents.Remove(target);
                ApplyFilter();
                SelectedIncident = null;
                StatusMessage = "Incident deleted";
            }
        }

        [RelayCommand]
        private async Task ExportToExcel()
        {
            var dialog = new SaveFileDialog { Filter = "Excel Files (*.xlsx)|*.xlsx", DefaultExt = ".xlsx", FileName = $"RiskIncidents_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx", Title = "Export to Excel" };
            if (dialog.ShowDialog() != true) return;
            try
            {
                var headers = new[] { "Code", "Title", "Device", "Severity", "Probability", "Risk Level", "Status", "Date", "Reported By" };
                var rows = FilteredIncidents.Select(i => new object[] {
                    i.IncidentCode, i.Title, i.DeviceName, i.Severity.ToString(), i.Probability.ToString(),
                    i.OverallRisk.ToString(), i.Status.ToString(), i.IncidentDate, i.ReportedBy
                });
                await _exportService.ExportToExcelAsync(dialog.FileName, "Risk Incidents", headers, rows);
                StatusMessage = $"📊 Exported to Excel: {Path.GetFileName(dialog.FileName)}";
                OpenFile(dialog.FileName);
            }
            catch (Exception ex) { StatusMessage = $"❌ Export error: {ex.Message}"; }
        }

        [RelayCommand]
        private async Task ExportToCsv()
        {
            var dialog = new SaveFileDialog { Filter = "CSV Files (*.csv)|*.csv", DefaultExt = ".csv", FileName = $"RiskIncidents_{DateTime.Now:yyyyMMdd_HHmmss}.csv", Title = "Export to CSV" };
            if (dialog.ShowDialog() != true) return;
            try
            {
                var headers = new[] { "Code", "Title", "Device", "Severity", "Probability", "Risk Level", "Status", "Date", "Reported By" };
                var rows = FilteredIncidents.Select(i => new object[] {
                    i.IncidentCode, i.Title, i.DeviceName, i.Severity.ToString(), i.Probability.ToString(),
                    i.OverallRisk.ToString(), i.Status.ToString(), i.IncidentDate, i.ReportedBy
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
            var dialog = new SaveFileDialog { Filter = "PDF Files (*.pdf)|*.pdf", DefaultExt = ".pdf", FileName = $"RiskIncidents_{DateTime.Now:yyyyMMdd_HHmmss}.pdf", Title = "Export to PDF" };
            if (dialog.ShowDialog() != true) return;
            try
            {
                var headers = new[] { "Code", "Title", "Device", "Severity", "Risk", "Status" };
                var rows = FilteredIncidents.Select(i => new object[] {
                    i.IncidentCode, i.Title.Length > 25 ? i.Title[..25] + "…" : i.Title,
                    i.DeviceName, i.Severity.ToString(), i.OverallRisk.ToString(), i.Status.ToString()
                });
                var summary = new (string, string)[] {
                    ("Open", OpenIncidentsCount.ToString()),
                    ("Critical", CriticalIncidentsCount.ToString()),
                    ("High Risk", HighRiskCount.ToString()),
                    ("Closed", ClosedCount.ToString())
                };
                await _exportService.ExportToPdfAsync(dialog.FileName, "Risk Incidents Report",
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
