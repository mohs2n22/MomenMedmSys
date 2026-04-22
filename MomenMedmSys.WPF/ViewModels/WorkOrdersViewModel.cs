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
    public partial class WorkOrdersViewModel : ViewModelBase
    {
        private readonly IWorkOrderService _workOrderService;
        private readonly IDeviceService _deviceService;
        private readonly IDialogService _dialogService;
        private readonly IExportService _exportService;
        private readonly Func<WorkOrderFormViewModel>? _formFactory;
        private MainViewModel? _mainVM;

        public WorkOrdersViewModel(IWorkOrderService workOrderService, IDeviceService deviceService, IDialogService dialogService, IExportService exportService, Func<WorkOrderFormViewModel>? formFactory = null)
        {
            _workOrderService = workOrderService;
            _deviceService = deviceService;
            _dialogService = dialogService;
            _exportService = exportService;
            _formFactory = formFactory;
            Title = "Work Orders";
            LoadWorkOrdersCommand.Execute(null);
        }

        public void SetMainViewModel(MainViewModel mainVM) { _mainVM = mainVM; }

        public ObservableCollection<WorkOrder> WorkOrders { get; } = new();
        public ObservableCollection<WorkOrder> FilteredWorkOrders { get; } = new();
        public ObservableCollection<MedicalDevice> Devices { get; } = new();

        [ObservableProperty] private WorkOrder? _selectedWorkOrder;
        [ObservableProperty] private int _openCount;
        [ObservableProperty] private int _overdueCount;
        [ObservableProperty] private int _emergencyCount;
        [ObservableProperty] private string _searchText = string.Empty;
        [ObservableProperty] private string _filterPriority = "All";
        [ObservableProperty] private string _filterStatus = "All";

        partial void OnSearchTextChanged(string value) => ApplyFilter();
        partial void OnFilterPriorityChanged(string value) => ApplyFilter();
        partial void OnFilterStatusChanged(string value) => ApplyFilter();

        [RelayCommand]
        private async Task LoadWorkOrders()
        {
            IsLoading = true;
            try
            {
                WorkOrders.Clear();
                var all = await _workOrderService.GetAllWorkOrdersAsync();
                foreach (var w in all) WorkOrders.Add(w);

                Devices.Clear();
                var devices = await _deviceService.GetAllDevicesAsync();
                foreach (var d in devices) Devices.Add(d);

                OpenCount = await _workOrderService.GetOpenWorkOrderCountAsync();
                OverdueCount = await _workOrderService.GetOverdueWorkOrderCountAsync();
                EmergencyCount = WorkOrders.Count(w => w.Priority == WorkOrderPriority.Emergency);

                ApplyFilter();
                StatusMessage = $"Loaded {WorkOrders.Count} work orders";
            }
            catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
            finally { IsLoading = false; }
        }

        private void ApplyFilter()
        {
            FilteredWorkOrders.Clear();
            var query = WorkOrders.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var s = SearchText.ToLower();
                query = query.Where(w => w.WorkOrderNumber.ToLower().Contains(s) || w.FaultDescription.ToLower().Contains(s) ||
                    (w.Device?.DeviceName ?? "").ToLower().Contains(s) || (w.AssignedTo ?? "").ToLower().Contains(s));
            }

            if (FilterPriority != "All" && FilterPriority != "All Priorities")
            {
                if (Enum.TryParse<WorkOrderPriority>(FilterPriority, out var p))
                    query = query.Where(w => w.Priority == p);
            }

            if (FilterStatus != "All" && FilterStatus != "All Statuses")
            {
                if (Enum.TryParse<WorkOrderStatus>(FilterStatus, out var st))
                    query = query.Where(w => w.Status == st);
            }

            foreach (var w in query) FilteredWorkOrders.Add(w);
        }

        [RelayCommand]
        private void AddWorkOrder()
        {
            if (_formFactory == null || _mainVM == null) return;
            var form = _formFactory();
            form.SetAddMode();
            _mainVM.NavigateTo(form);
        }

        [RelayCommand]
        private async Task EditWorkOrder(WorkOrder? wo)
        {
            if (_formFactory == null || _mainVM == null) return;
            var target = wo ?? SelectedWorkOrder;
            if (target == null) { await _dialogService.ShowMessageAsync("Select a work order to edit.", "No Selection"); return; }
            var form = _formFactory();
            form.SetEditMode(target);
            _mainVM.NavigateTo(form);
        }

        [RelayCommand]
        private async Task DeleteWorkOrder(WorkOrder? wo)
        {
            var target = wo ?? SelectedWorkOrder;
            if (target == null) return;
            var confirmed = await _dialogService.ShowConfirmAsync("Delete this work order?", "Confirm");
            if (confirmed)
            {
                await _workOrderService.DeleteWorkOrderAsync(target.Id);
                WorkOrders.Remove(target);
                ApplyFilter();
                SelectedWorkOrder = null;
                StatusMessage = "Work order deleted";
            }
        }

        [RelayCommand]
        private async Task ExportToExcel()
        {
            var dialog = new SaveFileDialog { Filter = "Excel Files (*.xlsx)|*.xlsx", DefaultExt = ".xlsx", FileName = $"WorkOrders_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx", Title = "Export to Excel" };
            if (dialog.ShowDialog() != true) return;
            try
            {
                var headers = new[] { "WO#", "Device", "Fault Description", "Priority", "Status", "Reported By", "Assigned To", "Report Date", "Est. Cost", "Actual Cost" };
                var rows = FilteredWorkOrders.Select(w => new object[] {
                    w.WorkOrderNumber, w.DeviceName, w.FaultDescription, w.Priority.ToString(), w.Status.ToString(),
                    w.ReportedBy, w.AssignedTo, w.ReportDate, w.EstimatedCost, w.ActualCost
                });
                await _exportService.ExportToExcelAsync(dialog.FileName, "Work Orders", headers, rows);
                StatusMessage = $"📊 Exported to Excel: {Path.GetFileName(dialog.FileName)}";
                OpenFile(dialog.FileName);
            }
            catch (Exception ex) { StatusMessage = $"❌ Export error: {ex.Message}"; }
        }

        [RelayCommand]
        private async Task ExportToCsv()
        {
            var dialog = new SaveFileDialog { Filter = "CSV Files (*.csv)|*.csv", DefaultExt = ".csv", FileName = $"WorkOrders_{DateTime.Now:yyyyMMdd_HHmmss}.csv", Title = "Export to CSV" };
            if (dialog.ShowDialog() != true) return;
            try
            {
                var headers = new[] { "WO#", "Device", "Fault Description", "Priority", "Status", "Reported By", "Assigned To", "Report Date", "Est. Cost", "Actual Cost" };
                var rows = FilteredWorkOrders.Select(w => new object[] {
                    w.WorkOrderNumber, w.DeviceName, w.FaultDescription, w.Priority.ToString(), w.Status.ToString(),
                    w.ReportedBy, w.AssignedTo, w.ReportDate, w.EstimatedCost, w.ActualCost
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
            var dialog = new SaveFileDialog { Filter = "PDF Files (*.pdf)|*.pdf", DefaultExt = ".pdf", FileName = $"WorkOrders_{DateTime.Now:yyyyMMdd_HHmmss}.pdf", Title = "Export to PDF" };
            if (dialog.ShowDialog() != true) return;
            try
            {
                var headers = new[] { "WO#", "Device", "Fault", "Priority", "Status", "Cost" };
                var rows = FilteredWorkOrders.Select(w => new object[] {
                    w.WorkOrderNumber, w.DeviceName, w.FaultDescription.Length > 25 ? w.FaultDescription[..25] + "…" : w.FaultDescription,
                    w.Priority.ToString(), w.Status.ToString(), w.EstimatedCost.ToString("C2")
                });
                var summary = new (string, string)[] {
                    ("Total", FilteredWorkOrders.Count.ToString()),
                    ("Open", OpenCount.ToString()),
                    ("Overdue", OverdueCount.ToString()),
                    ("Emergency", EmergencyCount.ToString())
                };
                await _exportService.ExportToPdfAsync(dialog.FileName, "Work Orders Report",
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
